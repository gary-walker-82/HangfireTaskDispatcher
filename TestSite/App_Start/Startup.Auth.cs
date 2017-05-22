using ExternalTaskStorage.Tasks;
using Hangfire;
using Hangfire.Console;
using Hangfire.Extension.TaskDispatcher.GlobalConfiguration;
using Ninject;
using Owin;
using System.Reflection;

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
               .UseTaskDispatcherPages(Assembly.GetAssembly(typeof(BaseTaskParameters)));

            app.UseHangfireDashboard();
            app.UseHangfireServer();

        }
    }
}