using System;
using System.Text;
using Hangfire.Extension.TaskDispatcher.Interfaces;

namespace Hangfire.Extension.TaskDispatcher.Converters
{
    public class EnumInputElement : InputElement, IInputElement<Enum>
    {
        public override string WriteElement(object taskParameters)
        {
            var values = Enum.GetValues(Property.PropertyType);
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($@"<select class=""form-control"" id=""{Id}"" name=""{Id}"">");
            foreach (var value in values)
            {
                stringBuilder.AppendLine($@"<option value=""{value}"">{value}</option>");
            }
            stringBuilder.AppendLine("</select>");
            return stringBuilder.ToString();
        }
    }
}