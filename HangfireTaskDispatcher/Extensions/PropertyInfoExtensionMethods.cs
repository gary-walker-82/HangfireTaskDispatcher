using System;
using System.Linq;
using System.Reflection;

namespace Hangfire.Extension.TaskDispatcher.Extensions
{
    public static class PropertyInfoExtensionMethods
    {
        public static TAttribute GetCustomAttribute<TAttribute>(this PropertyInfo property) where TAttribute : Attribute
        {
            return property.GetCustomAttributes(typeof(TAttribute), true).Cast<TAttribute>().FirstOrDefault();

        }

    }

  
}