using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Hangfire.Extension.TaskDispatcher.Interfaces;

namespace TestSite.Tasks
{
    public abstract  class BaseTaskParameters :ITaskParameters
    {
        public virtual string Queue => "default";

        public override string ToString()
        {
             var r = new Regex(@"
                (?<=[A-Z])(?=[A-Z][a-z]) |
                 (?<=[^A-Z])(?=[A-Z]) |
                 (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);
            return r.Replace(this.GetType().Name.Replace("TaskParameters", "")," ");
        }
    }

    public class TestTaskParameters :BaseTaskParameters
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