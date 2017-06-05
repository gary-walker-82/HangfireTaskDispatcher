using Hangfire.Common;
using Hangfire.Dashboard;
using Hangfire.Extension.TaskDispatcher.Extensions;
using Hangfire.Extension.TaskDispatcher.Helpers;
using Hangfire.Extension.TaskDispatcher.Interfaces;
using Hangfire.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hangfire.Extension.TaskDispatcher.Pages
{
    public static class TaskDetailsRoutes
    {
        public static void AddCommands<T>(T task, string pageHeader) where T : ITaskParameters
        {
            var route = $"{TasksPage.UrlRoute}/{task.Queue}/{pageHeader.Replace(" ", string.Empty)}";
            DashboardRoutes.Routes.Add(route, new CommandWithResponseDispatcher(context => ProcessForm(task, pageHeader, context)));
        }

        public static void AddCommands(Type unconstructedType, List<Type> genericTypeOptions, string pageHeader)
        {
            var makeGenericType = unconstructedType.MakeGenericType(genericTypeOptions.FirstOrDefault());
            var task = Activator.CreateInstance(makeGenericType) as ITaskParameters;

            var route = $"{TasksPage.UrlRoute}/{task.Queue}/{pageHeader.Replace(" ", string.Empty)}";

            DashboardRoutes.Routes.Add(route, new CommandWithResponseDispatcher(context =>
            {
                var typeName = Task.Run(() => context.Request.GetFormValuesAsync("objecttype")).Result.FirstOrDefault();
                var genericTypeParameter = genericTypeOptions.FirstOrDefault(x => x.Name == typeName);

                task = Activator.CreateInstance(unconstructedType.MakeGenericType(genericTypeParameter)) as ITaskParameters;
                return ProcessForm(task, pageHeader, context);
            }));
        }

        private static bool ProcessForm<T>(T task, string pageHeader, DashboardContext context) where T : ITaskParameters
        {
            var queue = task.Queue;
            task.PopulateFromRequest(context.Request);
            var schedule = Task.Run(() => context.Request.GetFormValuesAsync("schedule")).Result.FirstOrDefault();
            var cron = Task.Run(() => context.Request.GetFormValuesAsync("cron")).Result.FirstOrDefault();
            var action = Task.Run(() => context.Request.GetFormValuesAsync("action")).Result.FirstOrDefault();


            var methodInfo = typeof(ITaskDispatcher).GetMethod("Dispatch");
            var makeGenericMethod = methodInfo.MakeGenericMethod(task.GetType());
            var job = new Job(typeof(ITaskDispatcher), makeGenericMethod, task, null, null);
            var client = new BackgroundJobClient(context.Storage);
            var joblink = "";

            switch (action)
            {
                case "schedule":
                    var minutes = int.Parse(schedule);
                    joblink = new UrlHelper(context).JobDetails(client.Create(job, new ScheduledState(new TimeSpan(0, 0, minutes, 0))));
                    break;
                case "cron":
                    var manager = new RecurringJobManager(context.Storage);
                    try
                    {
                        manager.AddOrUpdate(pageHeader, job, cron, TimeZoneInfo.Utc, queue);
                        joblink = new UrlHelper(context).To("/recurring");

                    }
                    catch (Exception)
                    {
                        return false;
                    }
                    break;
                default:
                    joblink = new UrlHelper(context).JobDetails(client.Create(job, new EnqueuedState(queue)));
                    break;
            }
            context.Response.WriteAsync(joblink);
            return true;
        }
    }
}