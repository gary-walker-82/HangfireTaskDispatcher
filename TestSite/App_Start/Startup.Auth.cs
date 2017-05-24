﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hangfire;
using Hangfire.Console;
using Hangfire.Extension.TaskDispatcher.GlobalConfiguration;
using Hangfire.Extension.TaskDispatcher.Interfaces;
using Ninject;
using Owin;
using Tasks;

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
               .UseTaskDispatcherPages(kernel.GetAll<ITaskHandler>());

            app.UseHangfireDashboard();
            app.UseHangfireServer();

                  }
    }
}