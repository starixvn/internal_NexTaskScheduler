namespace NexTaskScheduler.AutoJobs.HRMTasks
{
    using System;
    using System.Threading.Tasks;
    using Hangfire;
    using Hangfire.Common;
    using Microsoft.Extensions.Options;
    using NexTaskScheduler.Core.Enum;
    using NexTaskScheduler.Core.Extentions;
    using NexTaskScheduler.Core.Models;
    using NexTaskScheduler.Interfaces.AutoJobs;
    using NexTaskScheduler.Service.Services.Interfaces;
    using RestSharp;

    public class ScheduledMessage : IJobSchedule
    {
        private readonly IRestClient webClient;
        private readonly IRecurringJobManager recurringJob;
        private readonly ILogService logService;
        private readonly IOptionsSnapshot<CronExpressionConfigs> configs;
        private readonly IOptionsSnapshot<NexHrmConfigs> hrmConfig;

        public ScheduledMessage(
            IRestClient webClient,
            IRecurringJobManager recurringJob,
            ILogService logService,
            IOptionsSnapshot<CronExpressionConfigs> configs,
            IOptionsSnapshot<NexHrmConfigs> hrmConfig)
        {
            this.webClient = webClient;
            this.recurringJob = recurringJob;
            this.logService = logService;
            this.configs = configs;
            this.hrmConfig = hrmConfig;
        }

        public void Initialize()
        {
            recurringJob.AddOrUpdate(MessageCategory.NexHrmScheduledMessage.GetDisplayName(),
                Job.FromExpression(() => RunJob(MessageCategory.NexHrmScheduledMessage)),
                configs.Value.ScheduledMessageSchedule,
                TimeZoneInfo.Local);
        }

        public async Task RunJob(MessageCategory categoryId)
        {
            var request = CreateRequestHeader(hrmConfig.Value.ScheduledMessageRoute, Method.POST);

            webClient.BaseUrl = new Uri(hrmConfig.Value.ApiUrl);
            await webClient.ExecuteTaskAsync(request);
            await logService.WriteCategoryMessageLog(categoryId);
        }

        private RestRequest CreateRequestHeader(string resource, Method method)
        {
            var request = new RestRequest(resource, method);
            return request;
        }
    }
}