using System.Reflection;

namespace Hangfire.Extension.TaskDispatcher.Interfaces
{
  

    public interface IInputElement
    {
        string Id { get; }
        string PlaceHolderText { get; }
        string HelpText { get; }
        string DisplayName { get; }
        bool ReadOnly { get; }
        PropertyInfo Property { get; set; }
        string WriteElementAndLabel(object taskParameters);
    }

    public interface IInputElement<T> : IInputElement{}
}