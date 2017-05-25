using Hangfire.Common;
using Hangfire.Dashboard;
using Hangfire.Extension.TaskDispatcher.Interfaces;
using Hangfire.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hangfire.Extension.TaskDispatcher.Pages
{
    public static class TaskDetailsRoutes
    {
        public static void AddCommands<T>(T task, string pageHeader) where T : ITaskParameters
        {
            var queue = task.Queue;
            var route = $"{TasksPage.UrlRoute}/{queue}/{pageHeader.Replace(" ", string.Empty)}";
            DashboardRoutes.Routes.AddCommand(route, context =>
            {
                var schedule = Task.Run(() => context.Request.GetFormValuesAsync("schedule")).Result.FirstOrDefault();
                var cron = Task.Run(() => context.Request.GetFormValuesAsync("cron")).Result.FirstOrDefault();
                var action = Task.Run(() => context.Request.GetFormValuesAsync("action")).Result.FirstOrDefault();

                task.PopulateFromRequest(context.Request);

                var methodInfo = typeof(ITaskDispatcher).GetMethod("Dispatch");
                var makeGenericMethod = methodInfo.MakeGenericMethod(task.GetType());
                var job = new Job(typeof(ITaskDispatcher), makeGenericMethod, task, null, null);
                var client = new BackgroundJobClient(context.Storage);

                switch (action)
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

        public static void AddCommands(Type unconstructedType, List<Type> genericTypeOptions, string pageHeader)
        {
            var makeGenericType = unconstructedType.MakeGenericType(genericTypeOptions.FirstOrDefault());
            var task = Activator.CreateInstance(makeGenericType) as ITaskParameters;
            var queue = task.Queue;
            var route = $"{TasksPage.UrlRoute}/{queue}/{pageHeader.Replace(" ", string.Empty)}";
            DashboardRoutes.Routes.AddCommand(route, context =>
            {
                var schedule = Task.Run(() => context.Request.GetFormValuesAsync("schedule")).Result.FirstOrDefault();
                var cron = Task.Run(() => context.Request.GetFormValuesAsync("cron")).Result.FirstOrDefault();
                var action = Task.Run(() => context.Request.GetFormValuesAsync("action")).Result.FirstOrDefault();
                var typeName = Task.Run(() => context.Request.GetFormValuesAsync("objecttype")).Result.FirstOrDefault();
                var genericTypeParameter = genericTypeOptions.FirstOrDefault(x => x.Name == typeName);

                task = Activator.CreateInstance(unconstructedType.MakeGenericType(genericTypeParameter)) as ITaskParameters;
                task.PopulateFromRequest(context.Request);

                var methodInfo = typeof(ITaskDispatcher).GetMethod("Dispatch");
                var makeGenericMethod = methodInfo.MakeGenericMethod(task.GetType());
                var job = new Job(typeof(ITaskDispatcher), makeGenericMethod, task, null, null);
                var client = new BackgroundJobClient(context.Storage);

                switch (action)
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