using Hangfire.Dashboard;
using Hangfire.Extension.TaskDispatcher.Extensions;
using Hangfire.Extension.TaskDispatcher.Helpers;
using Hangfire.Extension.TaskDispatcher.Interfaces;
using Hangfire.Extension.TaskDispatcher.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Hangfire.Extension.TaskDispatcher.GlobalConfiguration
{
    public static class GlobalConfigurationExtension
    {
        [Obsolete("You should now use use TaskDispatcherPagesOptions and set the handlers directly")]
        public static IGlobalConfiguration UseTaskDispatcherPages(this IGlobalConfiguration config,IEnumerable<ITaskHandler> handlers)
        {
            return config.UseTaskDispatcherPages(new TaskDispatcherPagesOptions() {TaskHandlers = handlers});
        }

        public static IGlobalConfiguration UseTaskDispatcherPages(this IGlobalConfiguration config, TaskDispatcherPagesOptions options)
        {
            options = options ?? new TaskDispatcherPagesOptions();
            CreateTopNavMenuItem();
            CreateScriptRoute();
            CreateWebViews(options);

            return config;
        }

        private static void CreateWebViews(TaskDispatcherPagesOptions options)
        {
            var enumerable =
                options.TaskHandlers.Select(
                    x =>
                        x.GetType()
                            .GetInterfaces()
                            .FirstOrDefault(y => y.IsConstructedGenericType)
                            .GetGenericArguments()
                            .FirstOrDefault()).ToList()
                            ;
            
            foreach (var taskTypeGroup in enumerable.GroupBy(x => x.Name).ToDictionary(x => x.Key, x => x.ToList()))
            {

                var taskType = taskTypeGroup.Value.FirstOrDefault();
                var task = Activator.CreateInstance(taskType) as ITaskParameters;
                TasksMenu.Tasks.Add(task);
                TasksMenu.AuthTasks.Add(task);
                if (taskTypeGroup.Value.Count() == 1)
                {
                    TaskDetailsRoutes.AddCommands(task, taskType.Name);

                    DashboardRoutes.Routes.AddRazorPage($"{TasksPage.UrlRoute}/{taskType.Name.Replace("`1", "")}",
                        x => new TaskDetailsPage(task,options));
                }
                else
                {
                    var types = taskTypeGroup.Value.Select(x => x.GenericTypeArguments.FirstOrDefault());
                    var unConstructedGenericType = taskType.Assembly.GetTypes().FirstOrDefault(x => x.Name == taskType.Name);
                    TaskDetailsRoutes.AddCommands(unConstructedGenericType, types.ToList(), taskType.Name.Replace("`1", ""));
                    DashboardRoutes.Routes.AddRazorPage($"{TasksPage.UrlRoute}/{taskType.Name.Replace("`1", "")}",
                       x => new TaskDetailsPage(task,options, types.ToList()));
                }
            }
        }

        private static void CreateScriptRoute()
        {
            DashboardRoutes.Routes.Add("/jsm",
                new EmbeddedResourceDispatcher(Assembly.GetAssembly(typeof(TasksPage)),
                    "Hangfire.Extension.TaskDispatcher.Content.Management.js"));
        }

        private static void CreateTopNavMenuItem()
        {
            DashboardRoutes.Routes.AddRazorPage(TasksPage.UrlRoute, x => new TasksPage());
            NavigationMenu.Items.Add(page => new MenuItem(TasksPage.Title, page.Url.To(TasksPage.UrlRoute))
            {
                Active = page.RequestPath.StartsWith(TasksPage.UrlRoute)
            });
        }
    }
}
