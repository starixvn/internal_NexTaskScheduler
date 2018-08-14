namespace NexTaskScheduler.AutoJobs.HRMTasks
{
    using System.Collections.Generic;
    using System.Linq;
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
    using NexTaskScheduler.Service.Services.Interfaces;

    public class RemindHRDeductLateInEarlyOutSchedule : IAutoReminder
    {
        private readonly INexbotService nexbotService;
        private readonly IBaseService baseService;
        private readonly IRecurringJobManager recurringJob;
        private readonly IOptionsSnapshot<HangfireConfigs> configs;

        public RemindHRDeductLateInEarlyOutSchedule(
            INexbotService nexbotService,
            IBaseService baseService,
            IRecurringJobManager recurringJob,
            IOptionsSnapshot<HangfireConfigs> configs)
        {
            this.nexbotService = nexbotService;
            this.baseService = baseService;
            this.recurringJob = recurringJob;
            this.configs = configs;
        }

        public void Initialize()
        {
            recurringJob.AddOrUpdate(MessageCategory.LateInEarlyOutWarningDeduct.GetDisplayName(),
                Job.FromExpression(() => SendMessages(MessageCategory.LateInEarlyOutWarningDeduct)),
                configs.Value.LateInEarlyOutDeduction);
        }

        public async Task SendMessages(MessageCategory categoryId)
        {
            var deductedEmployees = await baseService.GetDataAsync<LateInEarlyOutModel>(
                new GetLateInEarlyOutDuductionCriteria { Year = 2016 });

            if (deductedEmployees.Any())
            {
                var conversations = await nexbotService.GetRegisteredConversation(categoryId);

                if (conversations.Any())
                {
                    var message = GenerateMessage(deductedEmployees);
                    await SendMessageToConversation(conversations, message);
                }
            }
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
            var message = $"Hi All,{Constants.NewLine}";
            message += $"There is/are inform {deductedEmployees.Count()} employee(s) need to deduct balance for Late in/Early out: {Constants.NewLine}";
            foreach (var e in deductedEmployees)
            {
                string balanceCurrentYear = (e.BalanceCurrentYear / 60).ToString("0.0");
                message += $"_- {e.FullName} ({e.NickName}). Balance Current Year: {balanceCurrentYear} hour(s)._ {Constants.NewLine}";
            }
            message += "Please access https://hrm.nexcel.vn/report/lateinearlyout to review and take a deduction. Thanks.";
            return message;
        }
    }
}