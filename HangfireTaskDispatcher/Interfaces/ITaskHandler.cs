namespace Hangfire.Extension.TaskDispatcher.Interfaces
{
    public interface ITaskHandler { }
    public interface ITaskHandler<in T>: ITaskHandler where T: ITaskParameters
    {
        void Process(T taskParameters);
    }
}
