using Hangfire.Extension.TaskDispatcher.Interfaces;

namespace Hangfire.Extension.TaskDispatcher.Converters
{
    public class IntInputElement : InputElement, IInputElement<int>
    {
        public override string WriteElement(object taskParameters)
        {
            return $@"<input class=""form-control"" type=""number"" placeholder=""{DisplayName}"" id=""{Id}"" name=""{Id}"" value=""{GetValue(taskParameters)}"" {ReadOnlyString}>";
        }
    }
}