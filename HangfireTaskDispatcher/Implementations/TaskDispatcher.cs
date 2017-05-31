using System;
using System.Collections.Generic;
using Hangfire.Extension.TaskDispatcher.Interfaces;
using Hangfire.Server;

namespace Hangfire.Extension.TaskDispatcher.Implementations
{
    public class TaskDispatcher : ITaskDispatcher
    {
        private readonly List<ITaskHandler> _taskHandlers;

        public TaskDispatcher(List<ITaskHandler> taskHandlers)
        {
            _taskHandlers = taskHandlers;
        }

        public void Dispatch<T>(T taskParameters, PerformContext context, IJobCancellationToken cancellationToken) where T : ITaskParameters
        {
            if (taskParameters == null) throw new ArgumentNullException("taskParameters");

            var handler = (ITaskHandler<T>)_taskHandlers.Find(e => e is ITaskHandler<T>);
            if (handler == null) throw new Exception("not handler found for task");

            handler.Process(taskParameters);
        }
    }
}