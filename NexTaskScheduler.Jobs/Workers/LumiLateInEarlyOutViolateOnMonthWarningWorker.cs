namespace NexTaskScheduler.Jobs.Workers
{
    using System.Collections.Generic;
    using System.Net.Http;
    using Microsoft.Extensions.Options;
    using NexTaskScheduler.Core;
    using NexTaskScheduler.Core.Constant;
    using NexTaskScheduler.Core.Extentions;
    using NexTaskScheduler.Core.Models;

    public class LumiLateInEarlyOutViolateOnMonthWarningWorker : BaseWorker
    {
        public LumiLateInEarlyOutViolateOnMonthWarningWorker(
            IOptions<List<JobSetting>> jobSettings,
            IHttpClientFactory clientFactory,
            ILogServiceProvider logService)
                : base(logService, clientFactory)
        {
            jobSetting = JobHelper.GetJobConfig(jobSettings.Value, JobConstants.LumiLateInEarlyOutViolateOnMonthWarning);
        }
    }
}