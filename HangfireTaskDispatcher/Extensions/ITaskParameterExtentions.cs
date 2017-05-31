using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Hangfire.Dashboard;
using Hangfire.Extension.TaskDispatcher.Interfaces;

namespace Hangfire.Extension.TaskDispatcher.Extensions
{
    public static class ITaskParameterExtentions
    {
        public static void PopulateFromRequest(this ITaskParameters task, DashboardRequest request)
        {
            foreach (var property in task.GetType().GetProperties().Where(x => x.CanWrite))
            {
                SetProperty(request, property, task);
            }
        }

        private static void SetProperty(DashboardRequest request, PropertyInfo propertyInfo, ITaskParameters searchObject)
        {
            var value = Task.Run(() => request.GetFormValuesAsync(propertyInfo.Name)).Result.FirstOrDefault();

            if (value == null) return;

            var propertyType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ??
                               propertyInfo.PropertyType;
            var propertyValue = ConvertFromInvariantString(propertyType, value);

            propertyInfo.SetValue(searchObject, propertyValue, null);
        }

        private static object ConvertFromInvariantString(Type newT, string item)
        {
            var converter = TypeDescriptor.GetConverter(newT);
            return converter.IsValid(item)
                ? converter.ConvertFromInvariantString(item)
                : null;
        }
    }
}