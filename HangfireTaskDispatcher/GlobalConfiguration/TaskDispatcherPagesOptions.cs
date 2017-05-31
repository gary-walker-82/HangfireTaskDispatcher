using System;
using System.Collections.Generic;
using System.Linq;
using Hangfire.Extension.TaskDispatcher.Interfaces;

namespace Hangfire.Extension.TaskDispatcher.GlobalConfiguration
{
    public class TaskDispatcherPagesOptions
    {
        public IEnumerable<ITaskHandler> TaskHandlers { get; set; }

        public bool ShowQueueName { get; set; } = true;
        public bool ShowReadOnlyProperties { get; set; } = false;

        public TaskDispatcherPagesOptions()
        {
            TaskHandlers = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic)
                .SelectMany(a => a.GetTypes())
                .Where(x => x.IsAssignableFrom(typeof(ITaskHandler)) && x.IsAbstract == false)
                .Select(Activator.CreateInstance)
                .Cast<ITaskHandler>();

        }
    }
}