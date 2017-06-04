using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Hangfire.Dashboard;
using Hangfire.Extension.TaskDispatcher.Pages;
using Microsoft.Owin;
using Tasks;

namespace TestSite.Filters
{
    public class AuthTaskDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var owinContext = new OwinContext(context.GetOwinEnvironment());
            var identityName = owinContext.Authentication.User.Identity.Name;
            var authLevel = AuthRoles.Dev;
            TasksMenu.AuthTasks.Clear();
            foreach (var taskParameterse in TasksMenu.Tasks)
            {
                var taskAuth = taskParameterse as ITaskAuth;
                if (taskAuth == null) continue;
                if (taskAuth.AuthRoles.HasFlag(authLevel)) TasksMenu.AuthTasks.Add(taskParameterse);
            }
            return true;
        }
    }
}