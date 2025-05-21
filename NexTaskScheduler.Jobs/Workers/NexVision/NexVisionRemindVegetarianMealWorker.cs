namespace NexTaskScheduler.Jobs.Workers.NexVision
{
    using Microsoft.Extensions.Options;
    using NexTaskScheduler.Core;
    using NexTaskScheduler.Core.Constant;
    using NexTaskScheduler.Core.Extentions;
    using NexTaskScheduler.Core.Models;
    using System.Collections.Generic;
    using System.Net.Http;

    internal class NexVisionRemindVegetarianMealWorker : BaseWorker
    {
        public NexVisionRemindVegetarianMealWorker(
            IOptions<List<JobSetting>> jobSettings,
            IHttpClientFactory clientFactory,
            ILogServiceProvider logService)
                : base(logService, clientFactory)
        {
            jobSetting = JobHelper.GetJobConfig(jobSettings.Value, JobConstants.NexVisionRemindVegetarianMeal);
        }
    }
}
