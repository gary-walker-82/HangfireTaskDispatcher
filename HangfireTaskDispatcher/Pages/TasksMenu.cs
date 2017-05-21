using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Hangfire.Dashboard;

namespace Hangfire.Extension.TaskDispatcher.Pages
{
    public static class TasksMenu
    {
        public static IDictionary<string, SortedList<string,Func<RazorPage, MenuItem>>> Items =new SortedDictionary<string, SortedList<string,Func<RazorPage, MenuItem>>>();
    }
}
