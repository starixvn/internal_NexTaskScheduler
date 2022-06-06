namespace NexTaskScheduler.Jobs.Workers
{
    using System.Collections.Generic;
    using Microsoft.Extensions.Options;
    using NexTaskScheduler.Core;
    using NexTaskScheduler.Core.Constant;
    using NexTaskScheduler.Core.Extentions;
    using NexTaskScheduler.Core.Models;
    using RestSharp;

    public class LumiViolateDisciplineWarningWorker : BaseWorker
    {
        public LumiViolateDisciplineWarningWorker(
            IOptions<List<JobSetting>> jobSettings,
            IRestClient restClient,
            ILogServiceProvider logService)
                : base(logService, restClient)
        {
            jobSetting = JobHelper.GetJobConfig(jobSettings.Value, JobConstants.LumiViolateDisciplineWarning);
        }
    }
}
