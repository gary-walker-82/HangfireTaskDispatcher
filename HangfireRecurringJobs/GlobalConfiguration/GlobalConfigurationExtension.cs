using Hangfire.Common;
using Hangfire.Extension.TaskDispatcher.Interfaces;
using Hangfire.States;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WebGrease.Css.Extensions;

namespace Hangfire.Extension.RecurringJobs.GlobalConfiguration
{
	public static class GlobalConfigurationExtension
	{
		private static List<Type> _toList;

        public static IGlobalConfiguration UseRecurringJobBuilder(this IGlobalConfiguration config, JobBuilderOptions builderOptions)
		{
			if (builderOptions.JsonFileLocations.Any(string.IsNullOrWhiteSpace)) throw new ArgumentException("JsonFileLocation must have a value");
			CreateRecurringJobs(builderOptions);

			return config;
		}

	    public static IGlobalConfiguration UseStartUpJobBuilder(this IGlobalConfiguration config, JobBuilderOptions builderOptions)
	    {
	        if (builderOptions.JsonFileLocations.Any(string.IsNullOrWhiteSpace)) throw new ArgumentException("JsonFileLocation must have a value");
	        CreateStartUpJobs(builderOptions);

	        return config;
	    }

		private static void CreateStartUpJobs(JobBuilderOptions builderOptions)
		{
			var jobManager = new BackgroundJobClient(JobStorage.Current);
			builderOptions.JsonFileLocations.Select(GetFileContents)
							.SelectMany(JsonConvert.DeserializeObject<List<JobInfo>>)
							.ForEach(x=>CreateStartUpJob(x,jobManager));
		}

	    private static void CreateRecurringJobs(JobBuilderOptions builderOptions)
	    {
	        var jobManager = new RecurringJobManager(JobStorage.Current);
	        builderOptions.JsonFileLocations.Select(GetFileContents)
	            .SelectMany(JsonConvert.DeserializeObject<List<RecurringJobInfo>>)
	            .ToList()
	            .ForEach(x => AddOrUpdateRecurringJob(x, jobManager));
	    }

		private static string GetFileContents(string fileName)
		{
			var fullFileName = File.Exists(fileName) ? fileName : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
			return File.ReadAllText(fullFileName);
		}

		public static void AddOrUpdateRecurringJob(RecurringJobInfo jobInfo, IRecurringJobManager jobManager)
		{
			var taskType = Type.GetType(jobInfo.Type, AssemblyResolver, null);
			var taskParameters = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(jobInfo.Paramters), taskType) as ITaskParameters;

			jobManager.AddOrUpdate(jobInfo.Name, CreateJob(taskParameters), jobInfo.Cron, TimeZoneInfo.Utc, jobInfo.Queue ?? EnqueuedState.DefaultQueue);
		}

	    private static void CreateStartUpJob(JobInfo jobInfo,IBackgroundJobClient client)
	    {
	        var taskType = Type.GetType(jobInfo.Type, AssemblyResolver, null);
	        var taskParameters = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(jobInfo.Paramters), taskType) as ITaskParameters;
            client.Create(CreateJob(taskParameters),new EnqueuedState(taskParameters.Queue));
	    }

		private static Job CreateJob(ITaskParameters taskParameters)
		{
			var methodInfo = typeof(ITaskDispatcher).GetMethod("Dispatch");
			var makeGenericMethod = methodInfo.MakeGenericMethod(taskParameters.GetType());
			var job = new Job(typeof(ITaskDispatcher), makeGenericMethod, taskParameters, null, null);

			return job;
		}

		private static System.Reflection.Assembly AssemblyResolver(System.Reflection.AssemblyName assemblyName)
		{
			assemblyName.Version = null;
			return System.Reflection.Assembly.Load(assemblyName);
		}
	}
}
