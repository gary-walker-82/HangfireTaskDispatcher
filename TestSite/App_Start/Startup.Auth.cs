using System;
using Hangfire;
using Hangfire.Console;
using Hangfire.Dashboard;
using Hangfire.Extension.RecurringJobs.GlobalConfiguration;
using Hangfire.Extension.TaskDispatcher.GlobalConfiguration;
using Hangfire.Extension.TaskDispatcher.Interfaces;
using Ninject;
using Owin;
using System.Collections.Generic;
using System.Reflection;
using BLS.EmailSubscriptions.HangfireExtension.Data;
using BLS.EmailSubscriptions.HangfireExtension.GlobalConfiguration;
using Castle.Components.DictionaryAdapter;
using TestSite.Filters;

namespace TestSite
{
	public partial class Startup
	{
		public void ConfigureAuth(IAppBuilder app)
		{
			var kernel = new StandardKernel();
			kernel.Load(Assembly.GetAssembly(typeof(Startup)));
            
          	GlobalConfiguration.Configuration
				.UseSqlServerStorage("hangfire")
				.UseConsole()
				.UseNinjectActivator(kernel)
			    .UseEmailSubscriptionUi()
				.UseTaskDispatcherPages(new TaskDispatcherPagesOptions()
				{
					TaskHandlers = kernel.GetAll<ITaskHandler>(),
					AuthorizationFilters = new List<ITaskAuthorizationFilter>
					{
						new TaskAuthorizationFilter()
					},
					ShowQueueName = false,
					ShowReadOnlyProperties = true
				})
				.UseRecurringJobBuilder(new JobBuilderOptions(new List<string> { "jobDescriptions.Json", "jobDescriptions2.Json" }))
	            .UseStartUpJobBuilder(new JobBuilderOptions(new List<string> { "startUpJobDescriptions.Json" }));
		    
			var dashboardOptions = new DashboardOptions
			{
				Authorization = new List<IDashboardAuthorizationFilter>
				{
					new AuthTaskDashboardAuthorizationFilter()
				}
			};
			app.UseHangfireDashboard("/hangfire", dashboardOptions);
			app.UseHangfireServer()
			;

		}
	}
}