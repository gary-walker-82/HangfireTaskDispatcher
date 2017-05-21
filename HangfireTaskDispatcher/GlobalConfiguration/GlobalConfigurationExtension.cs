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
            CreateTopNavMenuItem();
            CreateScriptRoute();
            CreateWebViews(assembly);

            return config;
        }

        private static void CreateWebViews(Assembly assembly)
        {
            var tasks = assembly.GetTypes()
                .Where(x => typeof(ITaskParameters).IsAssignableFrom(x) && x.IsAbstract ==false)
                .Select(Activator.CreateInstance)
                .Cast<ITaskParameters>()
                .ToList();

            foreach (var task in tasks)
            {
                var taskType = task.GetType();
                var displayName = taskType.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? task.ToString();
                TaskDetailsRoutes.AddCommands(task, taskType.Name);
                if (!TasksMenu.Items.ContainsKey(task.Queue)) TasksMenu.Items.Add(task.Queue, new SortedList<string, Func<RazorPage, MenuItem>>());
                var menuItems = TasksMenu.Items[task.Queue];
                menuItems.Add(displayName,p => new MenuItem(displayName, p.Url.To($"{TasksPage.UrlRoute}/{taskType.Name}"))
                {
                    Active = p.RequestPath.StartsWith($"{TasksPage.UrlRoute}/{taskType.Name}")
                });

                DashboardRoutes.Routes.AddRazorPage($"{TasksPage.UrlRoute}/{taskType.Name}", x => new TaskDetailsPage(task));
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
