using Hangfire.Extension.TaskDispatcher.Interfaces;

namespace Hangfire.Extension.TaskDispatcher.Converters
{
    public class BoolInputElement : InputElement, IInputElement<bool>
    {
        public override string WriteElement(object taskParameters)
        {
            return $@"<div class=""btn-group"" id=""status"" data-toggle=""buttons"">
                    <label class=""btn btn-default btn-on btn-sm active"">
                  <input type =""radio"" value=""true"" name=""{Id}"">YES</label>
                  <label class=""btn btn-default btn-off btn-sm "">
                  <input type =""radio"" value=""false"" name=""{Id}"">NO</label>
                </div>";
        }
    }
}