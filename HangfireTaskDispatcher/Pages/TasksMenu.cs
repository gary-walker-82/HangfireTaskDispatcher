using System;
using System.Collections.Generic;
using System.ComponentModel;
using Hangfire.Dashboard;
using Hangfire.Extension.TaskDispatcher.Extensions;
using Hangfire.Extension.TaskDispatcher.Interfaces;

namespace Hangfire.Extension.TaskDispatcher.Pages
{
    public static class TasksMenu
    {
        public static IList<ITaskParameters> Tasks = new List<ITaskParameters>();
        public static IList<ITaskParameters> AuthTasks = new List<ITaskParameters>();
        public static IDictionary<string, SortedList<string,Func<RazorPage, MenuItem>>> Items =new SortedDictionary<string, SortedList<string,Func<RazorPage, MenuItem>>>();
        public static IDictionary<string, SortedList<string,Func<RazorPage, MenuItem>>> DisplayableTasks =new SortedDictionary<string, SortedList<string,Func<RazorPage, MenuItem>>>();

        public static IDictionary<string, SortedList<string, Func<RazorPage, MenuItem>>> CreateMenu()
        {
            Items = new SortedDictionary<string, SortedList<string, Func<RazorPage, MenuItem>>>();
            foreach (var task in AuthTasks)
            {
                var taskType = task.GetType();

                var displayName = taskType.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ??
                                  task.ToString();
                if (!Items.ContainsKey(task.Queue))
                    Items.Add(task.Queue, new SortedList<string, Func<RazorPage, MenuItem>>());
                var menuItems = Items[task.Queue];
                menuItems.Add(displayName,
                    p => new MenuItem(displayName, p.Url.To($"{TasksPage.UrlRoute}/{taskType.Name.Replace("`1", "")}"))
                    {
                        Active = p.RequestPath.StartsWith($"{TasksPage.UrlRoute}/{taskType.Name.Replace("`1", "")}")
                    });
            }

            return Items;
        }
    }
}
