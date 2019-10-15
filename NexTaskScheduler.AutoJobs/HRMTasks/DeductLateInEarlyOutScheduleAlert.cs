namespace NexTaskScheduler.AutoJobs.HRMTasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Core.Constant;
    using Hangfire;
    using Hangfire.Common;
    using Microsoft.Extensions.Options;
    using NexTaskScheduler.Core.Enum;
    using NexTaskScheduler.Core.Extentions;
    using NexTaskScheduler.Core.Models;
    using NexTaskScheduler.Core.Models.HRM;
    using NexTaskScheduler.Criteria.HRM;
    using NexTaskScheduler.Interfaces.AutoJobs;
    using NexTaskScheduler.Service.Services.Interfaces;

    public class DeductLateInEarlyOutScheduleAlert : IJobSchedule
    {
        private readonly INexbotService nexbotService;
        private readonly IBaseService baseService;
        private readonly ILogService logService;
        private readonly IRecurringJobManager recurringJob;
        private readonly IOptionsSnapshot<CronExpressionConfigs> configs;

        public DeductLateInEarlyOutScheduleAlert(
            INexbotService nexbotService,
            IBaseService baseService,
            ILogService logService,
            IRecurringJobManager recurringJob,
            IOptionsSnapshot<CronExpressionConfigs> configs)
        {
            this.nexbotService = nexbotService;
            this.baseService = baseService;
            this.logService = logService;
            this.recurringJob = recurringJob;
            this.configs = configs;
        }

        public void Initialize()
        {
            recurringJob.AddOrUpdate(MessageCategory.DeductLateInEarlyOut.GetDisplayName(),
                Job.FromExpression(() => RunJob(MessageCategory.DeductLateInEarlyOut)),
                configs.Value.LateInEarlyOutDeductSchedule,
                TimeZoneInfo.Local);
        }

        public async Task RunJob(MessageCategory categoryId)
        {
            var deductedEmployees = await baseService.GetDataAsync<LateInEarlyOutModel>(
                new GetLateInEarlyOutDuductionCriteria { Year = DateTime.Now.Year });

            if (deductedEmployees.Any())
            {
                var conversations = await nexbotService.GetRegisteredConversation(categoryId);

                if (conversations.Any())
                {
                    var message = GenerateMessage(deductedEmployees);
                    await SendMessageToConversation(conversations, message);
                }
            }

            await logService.WriteCategoryMessageLog(categoryId);
        }

        private async Task SendMessageToConversation(IEnumerable<string> conversations, string message)
        {
            foreach (var conversation in conversations)
            {
                await nexbotService.SendMessage(conversation, message);
            }
        }

        private string GenerateMessage(IEnumerable<LateInEarlyOutModel> deductedEmployees)
        {
            StringBuilder message = new StringBuilder();
            message.Append($"Hi All,{Constants.NewLine}");
            message.Append($"There is/are inform {deductedEmployees.Count()} employee(s) need to deduct balance for Late in/Early out: {Constants.NewLine}");
            foreach (var e in deductedEmployees)
            {
                string balanceCurrentYear = (e.BalanceCurrentYear / 60).ToString("0.0");
                message.Append($"_- {e.FullName} ({e.NickName}). Balance Current Year: {balanceCurrentYear} hour(s)._ {Constants.NewLine}");
            }
            message.Append("Please access https://hrm.nexcel.vn/report/lateinearlyout to review it and take a deduction. Thanks.");
            return message.ToString();
        }
    }
}