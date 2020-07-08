namespace NexTaskScheduler.AutoJobs.HRMTasks
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

    public class RemindMissingCheckInCheckOutSchedule : IJobSchedule
    {
        private readonly IRestClient webClient;
        private readonly IRecurringJobManager recurringJob;
        private readonly ILogService logService;
        private readonly IOptionsSnapshot<CronExpressionConfigs> configs;
        private readonly IOptionsSnapshot<NexHrmConfigs> hrmConfig;

        public RemindMissingCheckInCheckOutSchedule(
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
            recurringJob.AddOrUpdate(MessageCategory.MissingCheckInCheckOut.GetDisplayName(),
                Job.FromExpression(() => RunJob(MessageCategory.MissingCheckInCheckOut)),
                configs.Value.MissingCheckInCheckOutSchedule,
                TimeZoneInfo.Local);
        }

        public async Task RunJob(MessageCategory categoryId)
        {
            var request = CreateRequestHeader(hrmConfig.Value.ReminderMissingCheckInCheckOutScheduleRoute, Method.POST);
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