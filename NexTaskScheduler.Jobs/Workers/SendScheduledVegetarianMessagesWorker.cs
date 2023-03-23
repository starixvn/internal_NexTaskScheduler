using Microsoft.Extensions.Options;
using NexTaskScheduler.Core.Constant;
using NexTaskScheduler.Core.Extentions;
using NexTaskScheduler.Core.Models;
using NexTaskScheduler.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NexTaskScheduler.Jobs.Workers
{
    internal class SendScheduledVegetarianMessages : BaseWorker
    {
        public SendScheduledVegetarianMessages(
            IOptions<List<JobSetting>> jobSettings,
            IHttpClientFactory clientFactory,
            ILogServiceProvider logService)
                : base(logService, clientFactory)
        {
            jobSetting = JobHelper.GetJobConfig(jobSettings.Value, JobConstants.SendScheduledVegetarianMessages);
        }
    }
}
