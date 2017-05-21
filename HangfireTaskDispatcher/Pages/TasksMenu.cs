using System;
using System.Collections.Generic;
using Hangfire.Dashboard;

namespace Hangfire.Extension.TaskDispatcher.Pages
{
    public static class TasksMenu
    {
        public static Dictionary<string, List<Func<RazorPage, MenuItem>>> Items =new Dictionary<string, List<Func<RazorPage, MenuItem>>>();
    }
}
