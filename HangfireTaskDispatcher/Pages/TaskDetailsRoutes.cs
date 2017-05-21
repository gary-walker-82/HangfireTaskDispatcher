using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Hangfire.Common;
using Hangfire.Dashboard;
using Hangfire.Extension.TaskDispatcher.Interfaces;
using Hangfire.States;
using WebGrease.Css.Extensions;

namespace Hangfire.Extension.TaskDispatcher.Pages
{
    public static class ITaskParameterExtentions
    {
        public static void PopulateFromRequest(this ITaskParameters task, DashboardRequest request)
        {
            task.GetType()
                .GetProperties()
                .Where(x => x.CanWrite)
                .ForEach(x => SetProperty(request, x, task));
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

    public static class TaskDetailsRoutes
    {
        public static void AddCommands<T>(T task, string pageHeader) where T:ITaskParameters
        {
            var queue =task.Queue;
            var route = $"{TasksPage.UrlRoute}/{queue}/{pageHeader.Replace(" ", string.Empty)}";
            DashboardRoutes.Routes.AddCommand(route, context =>
            {
                var schedule = Task.Run(() => context.Request.GetFormValuesAsync("schedule")).Result.FirstOrDefault();
                var cron = Task.Run(() => context.Request.GetFormValuesAsync("cron")).Result.FirstOrDefault();
                var action = Task.Run(() => context.Request.GetFormValuesAsync("action")).Result.FirstOrDefault();

                task.PopulateFromRequest(context.Request);


                var methodInfo = typeof(ITaskDispatcher).GetMethod("Dispatch");
                var makeGenericMethod = methodInfo.MakeGenericMethod(task.GetType());
                var job = new Job(typeof(ITaskDispatcher), makeGenericMethod, task);
                var client = new BackgroundJobClient(context.Storage);

                switch(action)
                {
                    case "schedule":
                        var minutes = int.Parse(schedule);
                        return client.Create(job, new ScheduledState(new TimeSpan(0, 0, minutes, 0))) != string.Empty;
                    case "cron":
                        var manager = new RecurringJobManager(context.Storage);
                        try
                        {
                            manager.AddOrUpdate(pageHeader, job, cron, TimeZoneInfo.Utc, queue);
                        }
                        catch (Exception)
                        {
                            return false;
                        }
                        return true;
                    default:
                        return client.Create(job, new EnqueuedState(queue)) != string.Empty;
                }
            });
        }

    }
    
}