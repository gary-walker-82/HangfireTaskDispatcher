using System;

namespace Hangfire.Extension.TaskDispatcher.Attributes
{
	public class ErrorImplicationDetailsAttribute : Attribute
	{
		public string Details { get; set; }

		public ErrorImplicationDetailsAttribute(string details)
		{
			Details = details;
		}
	}
}