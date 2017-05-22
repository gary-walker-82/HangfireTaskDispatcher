using System.Text.RegularExpressions;

namespace Hangfire.Extension.TaskDispatcher.Interfaces
{
    public interface ITaskParameters
    {
        string Queue { get; }
    }

    public abstract class BaseBlsTaskParametersBaseTaskParameters : ITaskParameters
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
}
