using System.Collections.Generic;
using Hangfire;
using Hangfire.Console;
using Hangfire.Extension.TaskDispatcher.GlobalConfiguration;
using Ninject;
using Owin;
using System.Reflection;
using Hangfire.Dashboard;
using Hangfire.Extension.TaskDispatcher.Interfaces;
using Tasks;
using TestSite.Filters;

namespace TestSite
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetAssembly(typeof(Startup)));
            GlobalConfiguration.Configuration
                .UseSqlServerStorage("hangfire")
                .UseConsole()    
                .UseNinjectActivator(kernel)
                .UseTaskDispatcherPages(new TaskDispatcherPagesOptions()
                {
                    TaskHandlers = kernel.GetAll<ITaskHandler>(),
                    AuthorizationFilters = new List<ITaskAuthorizationFilter>
                    {
                        new TaskAuthorizationFilter()
                    },
                    ShowQueueName = false,
                    ShowReadOnlyProperties = true
                });

            var dashboardOptions = new DashboardOptions
            {
                Authorization = new List<IDashboardAuthorizationFilter>
                {
                    new AuthTaskDashboardAuthorizationFilter()
                }
            };
            app.UseHangfireDashboard("/hangfire", dashboardOptions);
            app.UseHangfireServer();
        }
    }
}