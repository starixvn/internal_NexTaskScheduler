using Microsoft.Extensions.Options;
using NexTaskScheduler.Core;
using NexTaskScheduler.Core.Constant;
using NexTaskScheduler.Core.Extentions;
using NexTaskScheduler.Core.Models;
using System.Collections.Generic;
using System.Net.Http;

namespace NexTaskScheduler.Jobs.Workers
{
    public class ReminderLateInEarlyOutMoreThanSixtyMinuteWorker : BaseWorker
    {
        public ReminderLateInEarlyOutMoreThanSixtyMinuteWorker(
            IOptions<List<JobSetting>> jobSettings,
            IHttpClientFactory clientFactory,
            ILogServiceProvider logService)
                : base(logService, clientFactory)
        {
            jobSetting = JobHelper.GetJobConfig(jobSettings.Value, JobConstants.ReminderLateInEarlyOutMoreThanSixtyMinute);
        }
    }
}
