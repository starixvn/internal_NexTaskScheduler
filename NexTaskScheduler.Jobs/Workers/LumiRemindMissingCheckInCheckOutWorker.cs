namespace NexTaskScheduler.Jobs.Workers
{
    using System.Collections.Generic;
    using System.Net.Http;
    using Microsoft.Extensions.Options;
    using NexTaskScheduler.Core;
    using NexTaskScheduler.Core.Constant;
    using NexTaskScheduler.Core.Extentions;
    using NexTaskScheduler.Core.Models;

    public class LumiRemindMissingCheckInCheckOutWorker : BaseWorker
    {
        public LumiRemindMissingCheckInCheckOutWorker(
            IOptions<List<JobSetting>> jobSettings,
            IHttpClientFactory clientFactory,
            ILogServiceProvider logService)
                : base(logService, clientFactory)
        {
            jobSetting = JobHelper.GetJobConfig(jobSettings.Value, JobConstants.LumiRemindMissingCheckInCheckOut);
        }
    }
}