using Newtonsoft.Json;

namespace Hangfire.Extension.RecurringJobs
{
	public class JobInfo
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("type")]
		public string Type { get; set; }
        
	    [JsonProperty("queue")]
		public string Queue { get; set; }

		[JsonProperty("data")]
		public object Paramters { get; set; }
	}

    public class RecurringJobInfo : JobInfo
    {
        [JsonProperty("cron")]
        public string Cron { get; set; }

    }
}
