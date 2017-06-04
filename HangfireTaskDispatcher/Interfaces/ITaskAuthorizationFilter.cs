using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hangfire.Dashboard;

namespace Hangfire.Extension.TaskDispatcher.Interfaces
{
   public  interface ITaskAuthorizationFilter
    {
        bool Authorize(DashboardContext context, ITaskParameters taskParameters);
    }

    public class TaskAuthorizationFilter : ITaskAuthorizationFilter
    {
        public bool Authorize(DashboardContext context, ITaskParameters taskParameters)
        {
            return new Random().Next(1) ==1;
        }
    }
}
