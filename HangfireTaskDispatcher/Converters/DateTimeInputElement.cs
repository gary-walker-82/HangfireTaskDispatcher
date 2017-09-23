using Hangfire.Extension.TaskDispatcher.Interfaces;
using System;

namespace Hangfire.Extension.TaskDispatcher.Converters
{
	public class DateTimeInputElement : InputElement, IInputElement<DateTime>
	{
		public override string WriteElement(object taskParameters)
		{
			return $@"
                     <div class='input-group date datetimepicker' id='{Id}_datetimepicker' >
                            <input type='text' class=""form-control"" name=""{Id}"" placeholder=""{DisplayName}"" {ReadOnlyString}/>
                            <span class=""input-group-addon"">
                                <span class=""glyphicon glyphicon-calendar""></span>
                            </span>
                        </div>
                   ";
		}
	}
}