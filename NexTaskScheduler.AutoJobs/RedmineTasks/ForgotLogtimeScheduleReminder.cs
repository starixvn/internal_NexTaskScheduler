namespace NexTaskScheduler.AutoJobs.RedmineTasks
{
    using System;
    using System.Threading.Tasks;
    using Hangfire;
    using Hangfire.Common;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using NexTaskScheduler.Core.Enum;
    using NexTaskScheduler.Core.Extentions;
    using NexTaskScheduler.Core.Models;
    using NexTaskScheduler.Interfaces.AutoJobs;
    using NexTaskScheduler.Service.Services.Interfaces;
    using RestSharp;

    public class ForgotLogtimeScheduleReminder : IJobSchedule
    {
        private readonly ILogService logService;
        private readonly IRecurringJobManager recurringJob;
        private readonly IOptionsSnapshot<CronExpressionConfigs> configs;
        private readonly IRestClient webClient;
        private readonly IOptionsSnapshot<NexHrmConfigs> hrmConfig;

        public ForgotLogtimeScheduleReminder(
            ILogService logService,
            IRecurringJobManager recurringJob,
            IRestClient webClient,
            IOptionsSnapshot<NexHrmConfigs> hrmConfig,
            IOptionsSnapshot<CronExpressionConfigs> configs)
        {
            this.logService = logService;
            this.recurringJob = recurringJob;
            this.configs = configs;
            this.hrmConfig = hrmConfig;
            this.webClient = webClient;
        }

        public void Initialize()
        {
            recurringJob.AddOrUpdate(MessageCategory.ForgotSpendTime.GetDisplayName(),
               Job.FromExpression(() => RunJob(MessageCategory.ForgotSpendTime)),
               configs.Value.RemindForgotLogtimeSchedule,
               TimeZoneInfo.Local);
        }

        public async Task RunJob(MessageCategory categoryId)
        {
            var request = CreateRequestHeader(hrmConfig.Value.ForgotSpendTime, Method.POST);
            webClient.BaseUrl = new Uri(hrmConfig.Value.ApiUrl);
            var tokenReponse = await webClient.ExecuteTaskAsync(GetToken());
            if (tokenReponse.IsSuccessful)
            {
                request.AddHeader("Authorization", JsonConvert.DeserializeObject(tokenReponse.Content).ToString());
                await webClient.ExecuteTaskAsync(request);
                await logService.WriteCategoryMessageLog(categoryId);
            }
        }

        private RestRequest CreateRequestHeader(string resource, Method method)
        {
            var request = new RestRequest(resource, method);
            return request;
        }

        private RestRequest GetToken()
        {
            var route = $"{hrmConfig.Value.AuthTokenRoute}?username={hrmConfig.Value.UserName}&password={hrmConfig.Value.Password}";
            var request = new RestRequest(route, Method.POST);
            return request;
        }
    }
}