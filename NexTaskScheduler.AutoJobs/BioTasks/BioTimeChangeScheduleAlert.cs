namespace NexTaskScheduler.AutoJobs.BioTasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Hangfire;
    using Hangfire.Common;
    using Microsoft.Extensions.Options;
    using NexTaskScheduler.Core.Constant;
    using NexTaskScheduler.Core.Enum;
    using NexTaskScheduler.Core.Extentions;
    using NexTaskScheduler.Core.Models;
    using NexTaskScheduler.Core.Models.Bio;
    using NexTaskScheduler.Criteria.BioStation;
    using NexTaskScheduler.Interfaces.AutoJobs;
    using NexTaskScheduler.Service.Services.Interfaces;

    public class BioTimeChangeScheduleAlert : IJobSchedule
    {
        private readonly INexbotService nexbotService;
        private readonly IBaseService baseService;
        private readonly ILogService logService;
        private readonly IRecurringJobManager recurringJob;
        private readonly IOptionsSnapshot<CronExpressionConfigs> configs;

        public BioTimeChangeScheduleAlert(IBaseService baseService,
            INexbotService nexbotService,
            ILogService logService,
            IRecurringJobManager recurringJob,
            IOptionsSnapshot<CronExpressionConfigs> configs)
        {
            this.baseService = baseService;
            this.nexbotService = nexbotService;
            this.logService = logService;
            this.recurringJob = recurringJob;
            this.configs = configs;
        }

        public void Initialize()
        {
            recurringJob.AddOrUpdate(MessageCategory.BioTimeChange.GetDisplayName(),
                Job.FromExpression(() => RunJob(MessageCategory.BioTimeChange)),
                configs.Value.BioTimeChangeSchedule,
                TimeZoneInfo.Local);
        }

        public async Task RunJob(MessageCategory categoryId)
        {
            var criteria = new GetWarningBioTimeChangeCriteria
            {
                FromDate = DateTime.Now.Date.AddDays(-1),
                ToDate = DateTime.Now.Date.AddDays(1)
            };
            var warningTimeChanges = await baseService.GetDataAsync<BioTimeChange>(criteria);

            if (warningTimeChanges.Any())
            {
                var conversationIds = await nexbotService.GetRegisteredConversation(categoryId).ConfigureAwait(false);

                if (conversationIds.Any())
                {
                    var message = GenerateMesssage(warningTimeChanges);
                    foreach (var conversationId in conversationIds)
                    {
                        await nexbotService.SendMessage(conversationId, message).ConfigureAwait(false);
                    }
                }
            }

            await logService.WriteCategoryMessageLog(categoryId);
        }

        private string GenerateMesssage(IEnumerable<BioTimeChange> timeChanges)
        {
            var message = new StringBuilder();
            message.Append($"Hi All, There is/are {timeChanges.Count()} change(s) of time: {Constants.NewLine}");
            foreach (var timeChange in timeChanges)
            {
                message.Append($"_- Device Id: {timeChange.DeviceId}. Device Name: {timeChange.DeviceName}. Time Changed: {timeChange.TimeChange.ToShortFormatedDate()}._ {Constants.NewLine}");
            }
            message.Append($"Please, review the change(s). Thanks");
            return message.ToString();
        }
    }
}