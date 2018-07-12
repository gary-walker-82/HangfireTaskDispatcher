using System.Collections.Generic;

namespace Hangfire.Extension.RecurringJobs.GlobalConfiguration
{
	public class JobBuilderOptions
	{
		public List<string> JsonFileLocations { get; set; }
		public JobBuilderOptions() { }
		public JobBuilderOptions(List<string> fileLocations)
		{
			JsonFileLocations = fileLocations;
		}
	}
    
}