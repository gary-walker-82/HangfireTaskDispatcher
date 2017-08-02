using System.Collections.Generic;

namespace Hangfire.Extension.RecurringJobs.GlobalConfiguration
{
	public class RecurringJobBuilderOptions
	{
		public List<string> JsonFileLocations { get; set; }
		public RecurringJobBuilderOptions() { }
		public RecurringJobBuilderOptions(List<string> fileLocations)
		{
			JsonFileLocations = fileLocations;
		}
	}
}