namespace NexTaskScheduler.AutoJobs.RedmineTasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Hangfire;
    using Hangfire.Common;
    using Microsoft.Extensions.Options;
    using NexTaskScheduler.Core.Constant;
    using NexTaskScheduler.Core.Enum;
    using NexTaskScheduler.Core.Extentions;
    using NexTaskScheduler.Core.Models;
    using NexTaskScheduler.Core.Models.Redmine;
    using NexTaskScheduler.Service.Services.Interfaces;

    public class ForgotLogtimeScheduleReminder : IAutoReminder
    {
        private readonly INexbotService nexbotService;
        private readonly IRedmineService redmineService;
        private readonly IRecurringJobManager recurringJob;
        private readonly IOptionsSnapshot<CronExpressionConfigs> configs;

        public ForgotLogtimeScheduleReminder(INexbotService nexbotService,
            IRedmineService redmineService,
            IRecurringJobManager recurringJob,
            IOptionsSnapshot<CronExpressionConfigs> configs)
        {
            this.nexbotService = nexbotService;
            this.redmineService = redmineService;
            this.recurringJob = recurringJob;
            this.configs = configs;
        }

        public void Initialize()
        {
            recurringJob.AddOrUpdate(MessageCategory.ForgotSpendTime.GetDisplayName(),
               Job.FromExpression(() => SendMessages(MessageCategory.ForgotSpendTime)),
               configs.Value.RemindForgotLogtimeSchedule);
        }

        public async Task SendMessages(MessageCategory categoryId)
        {
            var forgotLogtimeEmployees = await redmineService.GetForgotLogtimeEmployees();

            if (forgotLogtimeEmployees.Any())
            {
                await SendMessageToEmployees(forgotLogtimeEmployees);

                await SendMessageToGroups(forgotLogtimeEmployees, categoryId);
            }

            await nexbotService.WriteCategoryMessageLog(categoryId);
        }

        private async Task SendMessageToGroups(IEnumerable<RedmineUser> forgotLogtimeEmployees, MessageCategory categoryId)
        {
            var conversationIds = await nexbotService.GetRegisteredConversation(categoryId);

            if (conversationIds.Any())
            {
                string message = GenerateGroupMessage(forgotLogtimeEmployees);

                foreach (var conversationId in conversationIds)
                {
                    await nexbotService.SendMessage(conversationId, message);
                }
            }
        }

        private string GenerateGroupMessage(IEnumerable<RedmineUser> employees)
        {
            var toDate = DateTime.Now.AddDays(-1);
            var fromDate = toDate.AddDays(-6);
            string message = $"Hi All. There is/are {employees.Count()} employee(s) forgot log time on Redmine from {fromDate.ToString("dd, MMM, yyyy")} to {toDate.ToString("dd, MMM, yyyy")}: {Constants.NewLine}";

            foreach (var employee in employees)
            {
                string forgotDates = string.Join("; ", employee.ForgotLogtimeDates.Select(d => d.ToString("dd MMM, yyyy")));
                message += $"_- {employee.FullName} ({employee.NickName}) forgot log time on {employee}._ {Constants.NewLine}";
            }
            message += "Please notice that. Thanks ";

            return message;
        }

        private async Task SendMessageToEmployees(IEnumerable<RedmineUser> forgotLogtimeEmployees)
        {
            foreach (var employee in forgotLogtimeEmployees)
            {
                var message = GenerateMessage(employee);
                await nexbotService.SendMessage(employee.ConversationId, message);
            }
        }

        private static string GenerateMessage(RedmineUser employee)
        {
            var dates = string.Join(", ", employee.ForgotLogtimeDates.Select(d => d.ToString("dd, MMM, yyyy")));
            var message = $"Dear {employee.NickName}, {Constants.NewLine}";
            message += $"You have forgot log time on _{dates}_. {Constants.NewLine}";
            message += $"Please log time on https://nexdev.net/ or you can log time directly on https://hrm.nexcel.vn/ Dashboard. Thanks";
            return message;
        }
    }
}