using System.Text.RegularExpressions;
using Hangfire.Extension.TaskDispatcher.Interfaces;

namespace Hangfire.Extension.TaskDispatcher.Implementations
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

    public abstract class BaseTaskParameters<T> : BaseTaskParameters
    {
        public override string ToString()
        {
            return base.ToString().Replace("`1", "").Trim();
        }
    }
}