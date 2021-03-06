﻿using Hangfire.Common;
using Hangfire.Extension.TaskDispatcher.Interfaces;
using Hangfire.States;

namespace Hangfire.Extension.TaskDispatcher.Attributes
{   
    public class UseQueueFromTaskParametersAttribute : JobFilterAttribute, IElectStateFilter
    {
        public void OnStateElection(ElectStateContext context)
        {
            var enqueuedState = context.CandidateState as EnqueuedState;
            if (enqueuedState == null) return;
            var cmd = context.BackgroundJob.Job.Args[0] as ITaskParameters;
            enqueuedState.Queue = !string.IsNullOrWhiteSpace(cmd?.Queue)
                ? cmd.Queue
                : "default";
        }
    }
}
