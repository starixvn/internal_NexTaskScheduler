namespace NexTaskScheduler.Jobs.Workers.NexVision
{
    using System.Collections.Generic;
    using System.Net.Http;
    using Microsoft.Extensions.Options;
    using NexTaskScheduler.Core;
    using NexTaskScheduler.Core.Constant;
    using NexTaskScheduler.Core.Extentions;
    using NexTaskScheduler.Core.Models;

    public class NexVisionSkypeAccountAutoMappingWorker : BaseWorker
    {
        public NexVisionSkypeAccountAutoMappingWorker(
            IOptions<List<JobSetting>> jobSettings,
            ILogServiceProvider logService,
            IHttpClientFactory clientFactory) : base(logService, clientFactory)
        {
            jobSetting = JobHelper.GetJobConfig(jobSettings.Value, JobConstants.NexVisionSkypeAccountAutoMapping);
        }
    }
}