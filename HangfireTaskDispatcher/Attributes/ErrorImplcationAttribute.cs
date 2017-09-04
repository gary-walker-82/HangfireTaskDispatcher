using System;

namespace Hangfire.Extension.TaskDispatcher.Attributes
{
	public class ErrorImplcationAttribute : Attribute
	{
		public string Errorimplication { get; set; };
		public ErrorImplcationAttribute(string errorimplication)
		{
			Errorimplication = errorimplication;
		}
	}
}