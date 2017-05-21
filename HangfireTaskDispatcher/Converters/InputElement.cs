using System.Reflection;
using System.Text.RegularExpressions;
using Hangfire.Extension.TaskDispatcher.Interfaces;

namespace Hangfire.Extension.TaskDispatcher.Converters
{
    public abstract class InputElement : IInputElement
    {
        private Regex r = new Regex(@"
                (?<=[A-Z])(?=[A-Z][a-z]) |
                 (?<=[^A-Z])(?=[A-Z]) |
                 (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);

        public abstract string WriteElement(object taskParameters);

        public virtual string Id => Property?.Name;
        public virtual string PlaceHolderText => DisplayName;
        public virtual string HelpText => Property?.Name;
        public virtual string DisplayName => r.Replace(Id, " ");
        public virtual bool ReadOnly => !Property.CanWrite;
        protected virtual string ReadOnlyString => ReadOnly ? @"disabled class=""disabled""" : string.Empty;

        public PropertyInfo Property { get; set; }
        
        protected virtual string GetValue(object taskParameters)
        {
            var value = string.Empty;
            if (ReadOnly)
            {
                value = Property.GetValue(taskParameters).ToString();
            }
            return value;
        }

        public virtual string WriteElementAndLabel(object taskParameters)
        {
            return $@"<div class=""form-group row"">
                        <label for=""{Id}"" class=""col-xs-3 col-form-label"">{DisplayName}</label>
                        <div class=""col-xs-9"">{WriteElement(taskParameters)}</div></div>";
        }
    }
}