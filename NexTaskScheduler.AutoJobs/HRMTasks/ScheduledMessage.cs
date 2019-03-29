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
    using RestSharp;

    public class ScheduledMessage : IJobSchedule
    {
        private readonly IRestClient webClient;
        private readonly IRecurringJobManager recurringJob;
        private readonly IOptionsSnapshot<CronExpressionConfigs> configs;
        private readonly IOptionsSnapshot<NexHrmConfigs> hrmConfig;

        public ScheduledMessage(
            IRestClient webClient,
            IRecurringJobManager recurringJob,
            IOptionsSnapshot<CronExpressionConfigs> configs,
            IOptionsSnapshot<NexHrmConfigs> hrmConfig)
        {
            this.webClient = webClient;
            this.recurringJob = recurringJob;
            this.configs = configs;
            this.hrmConfig = hrmConfig;
        }

        public void Initialize()
        {
            recurringJob.AddOrUpdate(MessageCategory.NexHrmScheduledMessage.GetDisplayName(),
                Job.FromExpression(() => RunJob(MessageCategory.NexGoalAutoJobSchedule)),
                configs.Value.NexGoalAutoJobSchedule,
                TimeZoneInfo.Local);
        }

        public async Task RunJob(MessageCategory categoryId)
        {
            var request = CreateRequestHeader("/ScheduleMessage", Method.PUT);

            webClient.BaseUrl = new Uri(hrmConfig.Value.ApiUrl);
            await webClient.ExecuteTaskAsync(request);
        }

        private RestRequest CreateRequestHeader(string resource, Method method)
        {
            var request = new RestRequest(resource, method);
            return request;
        }
    }
}