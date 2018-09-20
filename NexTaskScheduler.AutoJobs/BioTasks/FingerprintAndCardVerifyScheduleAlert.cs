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
    using NexTaskScheduler.Service.Services.Interfaces;

    public class FingerprintAndCardVerifyScheduleAlert : IAutoReminder
    {
        private readonly INexbotService nexbotService;
        private readonly IBaseService baseService;
        private readonly IRecurringJobManager recurringJob;
        private readonly IOptionsSnapshot<CronExpressionConfigs> configs;

        public FingerprintAndCardVerifyScheduleAlert(IBaseService baseService,
            INexbotService nexbotService,
            IRecurringJobManager recurringJob,
            IOptionsSnapshot<CronExpressionConfigs> configs)
        {
            this.nexbotService = nexbotService;
            this.baseService = baseService;
            this.recurringJob = recurringJob;
            this.configs = configs;
        }

        public void Initialize()
        {
            recurringJob.AddOrUpdate(MessageCategory.BioFingerprintAndCard.GetDisplayName(),
               Job.FromExpression(() => SendMessages(MessageCategory.BioFingerprintAndCard)),
               configs.Value.FingerprintAndCardCheckSchedule,
               TimeZoneInfo.Local);
        }

        public async Task SendMessages(MessageCategory categoryId)
        {
            var members = await baseService.GetDataAsync<FingerprintAndCardSummary>(
                new GetPendingFingerprintAndCardCriteria()
                ).ConfigureAwait(false);

            if (members.Any())
            {
                var conversationIds = await nexbotService.GetRegisteredConversation(categoryId).ConfigureAwait(false);

                if (conversationIds.Any())
                {
                    var message = GenerateMesssage(members);
                    foreach (var conversationId in conversationIds)
                    {
                        await nexbotService.SendMessage(conversationId, message).ConfigureAwait(false);
                    }
                }
            }

            await nexbotService.WriteCategoryMessageLog(categoryId);
        }

        private static string GenerateMesssage(IEnumerable<FingerprintAndCardSummary> members)
        {
            StringBuilder message = new StringBuilder();
            message.Append($"Hi All, There is/are {members.Count()} new registered fingerprint(s) and/or card(s) waiting to be approved:{Constants.NewLine}");
            foreach (var member in members)
            {
                string typeToApprove = member.IsFingerprintWarning ? "**Fingerprint**" : string.Empty;
                typeToApprove += member.IsFingerprintWarning && member.IsCardWarning ? " and **Card**" : string.Empty;
                typeToApprove += member.IsCardWarning && !member.IsFingerprintWarning ? "**Card**" : string.Empty;

                message.Append($"_- {member.FullName} ({member.NickName})'s {typeToApprove}._ {Constants.NewLine}");
            }
            var hrmLink = "https://hrm.nexcel.vn/fingerprintscanner";
            message.Append($"Please, access {hrmLink} to review and take appropriate action. Thanks");
            return message.ToString();
        }
    }
}