using System;
using System.Linq;

namespace Hangfire.Extension.TaskDispatcher.Extensions
{
    public static class TypeExtensionMethods
    { 
        public static TAttribute GetCustomAttribute<TAttribute>(this Type type) where TAttribute : Attribute
        {
            return type.GetCustomAttributes(typeof(TAttribute), true).Cast<TAttribute>().FirstOrDefault();
        }

    }
}