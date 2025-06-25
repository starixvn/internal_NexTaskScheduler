using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Options;
using NexTaskScheduler.Core;
using NexTaskScheduler.Core.Constant;
using NexTaskScheduler.Core.Extentions;
using NexTaskScheduler.Core.Models;

namespace NexTaskScheduler.Jobs.Workers;

public class RemindCancelMealWorker : BaseWorker
{
    public RemindCancelMealWorker(
        IOptions<List<JobSetting>> jobSettings,
        IHttpClientFactory clientFactory,
        ILogServiceProvider logService)
        : base(logService, clientFactory)
    {
        jobSetting = JobHelper.GetJobConfig(jobSettings.Value, JobConstants.RemindCancelMeal);
    }
}