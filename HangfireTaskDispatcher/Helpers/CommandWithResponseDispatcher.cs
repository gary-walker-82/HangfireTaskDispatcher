using System;
using System.Net;
using System.Threading.Tasks;
using Hangfire.Dashboard;

namespace Hangfire.Extension.TaskDispatcher.Helpers
{
    internal class CommandWithResponseDispatcher : IDashboardDispatcher
    {
        private readonly Func<DashboardContext, bool> _command;

        public CommandWithResponseDispatcher(Func<DashboardContext, bool> command)
        {
            _command = command;
        }

        public Task Dispatch(DashboardContext context)
        {
            var request = context.Request;
            var response = context.Response;

            if (!"POST".Equals(request.Method, StringComparison.OrdinalIgnoreCase))
            {
                response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                return Task.FromResult(false);
            }

            if (_command(context))
            {
                response.StatusCode = (int)HttpStatusCode.OK;
            }
            else
            {
                response.StatusCode = 422;
            }

            return Task.FromResult(true);
        }
    }
}