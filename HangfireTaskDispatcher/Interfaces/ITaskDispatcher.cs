using System.ComponentModel;
using Hangfire.Extension.TaskDispatcher.Attributes;

namespace Hangfire.Extension.TaskDispatcher.Interfaces
{
    [UseQueueFromTaskParameters]
    [AutomaticRetry]
    public interface ITaskDispatcher
    {
        [DisplayName("{0}")]
        void Dispatch<T>(T taskParameters) where T:ITaskParameters;
    }
}
