using Hangfire.Extension.TaskDispatcher.Interfaces;

namespace Hangfire.Extension.TaskDispatcher.Converters
{
    public class StringInputElement : InputElement, IInputElement<string>
    {
        public override string WriteElement(object taskParameters)
        {
            return $@"<input class=""form-control"" type=""text"" placeholder=""{DisplayName}"" id=""{Id}"" name=""{Id}"" value=""{GetValue(taskParameters)}"" {ReadOnlyString}>";
        }
    }
}