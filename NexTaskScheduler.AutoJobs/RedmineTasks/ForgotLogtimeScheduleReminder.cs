namespace NexTaskScheduler.AutoJobs.RedmineTasks
{
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
                foreach (var employee in forgotLogtimeEmployees)
                {
                    var message = GenerateMessage(employee);
                    await nexbotService.SendMessage(employee.ConversationId, message);
                }
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