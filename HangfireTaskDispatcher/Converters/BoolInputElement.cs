using System.Reflection;
using Hangfire.Extension.TaskDispatcher.Interfaces;

namespace Hangfire.Extension.TaskDispatcher.Converters
{
    public class BoolInputElement : InputElement, IInputElement<bool>
    {
        public override string WriteElement(object taskParameters)
        {
            var value = Property.GetValue(taskParameters);
            var activeString = value == null ? "" : "active";
            var checkedString = value == null ? "" : "checked";
            return $@"<div class=""btn-group"" id=""status"" data-toggle=""buttons"">
                    <label class=""btn btn-default btn-on btn-sm"" {ReadOnlyString}>
                  <input type =""radio"" value=""true"" name=""{Id}"" {ReadOnlyString}>YES</label>
                  <label class=""btn btn-default btn-off btn-sm {activeString}"" {ReadOnlyString}>
                  <input type =""radio"" value=""false"" name=""{Id}"" {ReadOnlyString} {checkedString}>NO</label>
                </div>";
        }
    }
}