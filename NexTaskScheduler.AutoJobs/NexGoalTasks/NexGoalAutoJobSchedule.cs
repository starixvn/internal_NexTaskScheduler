namespace NexTaskScheduler.AutoJobs.NexGoalTasks
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

    public class NexGoalAutoJobSchedule : IJobSchedule
    {
        private readonly IRestClient webClient;
        private readonly IRecurringJobManager recurringJob;
        private readonly IOptionsSnapshot<CronExpressionConfigs> configs;
        private readonly IOptionsSnapshot<NexGoalConfigs> nexGoalConfig;

        public NexGoalAutoJobSchedule(
            IRestClient webClient,
            IRecurringJobManager recurringJob,
            IOptionsSnapshot<CronExpressionConfigs> configs,
            IOptionsSnapshot<NexGoalConfigs> nexGoalConfig)
        {
            this.webClient = webClient;
            this.recurringJob = recurringJob;
            this.configs = configs;
            this.nexGoalConfig = nexGoalConfig;
        }

        public void Initialize()
        {
            recurringJob.AddOrUpdate(MessageCategory.NexGoalAutoJobSchedule.GetDisplayName(),
                Job.FromExpression(() => RunJob(MessageCategory.NexGoalAutoJobSchedule)),
                configs.Value.NexGoalAutoJobSchedule,
                TimeZoneInfo.Local);
        }

        public async Task RunJob(MessageCategory categoryId)
        {
            var request = CreateRequestHeader("/backgroundjob", Method.GET);

            webClient.BaseUrl = new Uri(nexGoalConfig.Value.NexGoalJobURL);
            var result = await webClient.ExecuteTaskAsync(request);
        }

        private RestRequest CreateRequestHeader(string resource, Method method)
        {
            var request = new RestRequest(resource, method);
            return request;
        }
    }
}
