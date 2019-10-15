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

    public class ReSendErrorMessage : IJobSchedule
    {
        private readonly IRestClient webClient;
        private readonly IRecurringJobManager recurringJob;
        private readonly ILogService logService;
        private readonly IOptionsSnapshot<CronExpressionConfigs> configs;
        private readonly IOptionsSnapshot<NexbotConfigs> nexBotConfig;

        public ReSendErrorMessage(
            IRestClient webClient,
            IRecurringJobManager recurringJob,
            ILogService logService,
            IOptionsSnapshot<CronExpressionConfigs> configs,
            IOptionsSnapshot<NexbotConfigs> nexBotConfig)
        {
            this.webClient = webClient;
            this.recurringJob = recurringJob;
            this.logService = logService;
            this.configs = configs;
            this.nexBotConfig = nexBotConfig;
        }

        public void Initialize()
        {
            recurringJob.AddOrUpdate(MessageCategory.ResendErrorMessage.GetDisplayName(),
                Job.FromExpression(() => RunJob(MessageCategory.ResendErrorMessage)),
                configs.Value.ReSendErrorMessageSchedule,
                TimeZoneInfo.Local);
        }

        public async Task RunJob(MessageCategory categoryId)
        {
            var request = CreateRequestHeader(nexBotConfig.Value.ReSendErrorMessage, Method.GET);

            webClient.BaseUrl = new Uri(nexBotConfig.Value.BotServiceUrl);
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