using Hangfire.Extension.TaskDispatcher.Interfaces;
using System;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace ExternalTaskStorage.Tasks
{
    public abstract class BaseTaskParameters : ITaskParameters
    {
        public virtual string Queue => "default";

        public override string ToString()
        {
            var r = new Regex(@"
                (?<=[A-Z])(?=[A-Z][a-z]) |
                 (?<=[^A-Z])(?=[A-Z]) |
                 (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);
            return r.Replace(this.GetType().Name.Replace("TaskParameters", ""), " ");
        }
    }

    public enum Mytype
    {
        option1,
        option2
    }
    [DisplayName("Task Two")]
    [Description("This is a description")]
    public class T2TaskParameters : BaseTaskParameters
    {
        public override string Queue => "newqueue";
        public string Name { get; set; }
        public bool? Option { get; set; }
        public DateTime? Date { get; set; }
        public string SomethingElse { get; set; }
        public Mytype myenum { get; set; }
        public long? LongNumber { get; set; }
        public Guid? GuidId { get; set; }
        public decimal? DecimalValue { get; set; }
        public double? DoubleValue { get; set; }
    }

    public class TestTaskParameters : BaseTaskParameters
    {
        public string MyName { get; set; }
    }

    [DisplayName("second task")]
    [Description("this is some details on the description")]
    public class Test1TaskParameters : BaseTaskParameters
    {
        public int MyNumber { get; set; }
    }
}