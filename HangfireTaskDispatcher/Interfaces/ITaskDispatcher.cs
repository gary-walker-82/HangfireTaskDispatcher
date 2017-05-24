using Hangfire.Extension.TaskDispatcher.Attributes;
using Hangfire.Server;
using System.ComponentModel;

namespace Hangfire.Extension.TaskDispatcher.Interfaces
{
    [UseQueueFromTaskParameters]
    [AutomaticRetry]
    public interface ITaskDispatcher
    {
        [DisplayName("{0}")]
        void Dispatch<T>(T taskParameters, PerformContext context, IJobCancellationToken cancellationToken) where T : ITaskParameters;

    }
}
