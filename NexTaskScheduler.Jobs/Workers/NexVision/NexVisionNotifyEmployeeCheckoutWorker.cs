namespace NexTaskScheduler.Jobs.Workers.NexVision
{
    using System.Collections.Generic;
    using System.Net.Http;
    using Microsoft.Extensions.Options;
    using NexTaskScheduler.Core;
    using NexTaskScheduler.Core.Constant;
    using NexTaskScheduler.Core.Extentions;
    using NexTaskScheduler.Core.Models;

    public class NexVisionNotifyEmployeeCheckoutWorker : BaseWorker
    {
        public NexVisionNotifyEmployeeCheckoutWorker(
       IOptions<List<JobSetting>> jobSettings,
           IHttpClientFactory clientFactory,
           ILogServiceProvider logService)
               : base(logService, clientFactory)
        {
            jobSetting = JobHelper.GetJobConfig(jobSettings.Value, JobConstants.NexVisionNotifyEmployeeCheckOut);
        }
    }
}