using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Hangfire.Dashboard;
using Hangfire.Extension.TaskDispatcher.Extensions;
using Hangfire.Extension.TaskDispatcher.Helpers;
using Hangfire.Extension.TaskDispatcher.Interfaces;
using Hangfire.Extension.TaskDispatcher.Pages;

namespace Hangfire.Extension.TaskDispatcher.GlobalConfiguration
{
    public static class GlobalConfigurationExtension
    {
        public static IGlobalConfiguration UseTaskDispatcherPages(this IGlobalConfiguration config, Assembly assembly)
        {
            var handlers =
                assembly.GetTypes()
                    .Where(x => x.IsAssignableFrom(typeof(ITaskHandler)) && x.IsAbstract == false)
                    .Select(Activator.CreateInstance)
                    .Cast<ITaskHandler>();
            return config.UseTaskDispatcherPages(handlers);
        }

        public static IGlobalConfiguration UseTaskDispatcherPages(this IGlobalConfiguration config,
            IEnumerable<ITaskHandler> handlers)
        {
            CreateTopNavMenuItem();
            CreateScriptRoute();
            CreateWebViews(handlers);

            return config;
        }

        private static void CreateWebViews(IEnumerable<ITaskHandler> handlers)
        {

            var enumerable =
                handlers.Select(
                    x =>
                        x.GetType()
                            .GetInterfaces()
                            .FirstOrDefault(y => y.IsConstructedGenericType)
                            .GetGenericArguments()
                            .FirstOrDefault()).ToList()
                            ;
            var taskHandler = handlers.LastOrDefault();
            var lastOrDefault = taskHandler.GetType().Assembly;
            var firstOrDefault = lastOrDefault.GetTypes().FirstOrDefault(x => x.Name == taskHandler.GetType().Name);
            
            foreach (var taskTypeGroup in enumerable.GroupBy(x => x.Name).ToDictionary(x => x.Key, x => x.ToList()))
            {
                
                    var taskType = taskTypeGroup.Value.FirstOrDefault();
                    var task = Activator.CreateInstance(taskType) as ITaskParameters;
                    var displayName = taskType.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ??
                                      task.ToString();
                    if (!TasksMenu.Items.ContainsKey(task.Queue))
                        TasksMenu.Items.Add(task.Queue, new SortedList<string, Func<RazorPage, MenuItem>>());
                    var menuItems = TasksMenu.Items[task.Queue];
                    menuItems.Add(displayName,
                        p => new MenuItem(displayName, p.Url.To($"{TasksPage.UrlRoute}/{taskType.Name.Replace("`1", "")}"))
                        {
                            Active = p.RequestPath.StartsWith($"{TasksPage.UrlRoute}/{taskType.Name.Replace("`1", "")}")
                        });

                if (taskTypeGroup.Value.Count() == 1)
                {
                    TaskDetailsRoutes.AddCommands(task, taskType.Name);

                    DashboardRoutes.Routes.AddRazorPage($"{TasksPage.UrlRoute}/{taskType.Name.Replace("`1", "")}",
                        x => new TaskDetailsPage(task));
                }
                else
                {
                    var typeses = taskTypeGroup.Value.Select(x=>x.GenericTypeArguments.FirstOrDefault());
                    var unConstructedGenericType = taskType.Assembly.GetTypes().FirstOrDefault(x=>x.Name == taskType.Name);
                    TaskDetailsRoutes.AddCommands(unConstructedGenericType, typeses.ToList(), taskType.Name.Replace("`1", ""));
                    DashboardRoutes.Routes.AddRazorPage($"{TasksPage.UrlRoute}/{taskType.Name.Replace("`1","")}",
                       x => new TaskDetailsPage(task, typeses.ToList() ));


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
