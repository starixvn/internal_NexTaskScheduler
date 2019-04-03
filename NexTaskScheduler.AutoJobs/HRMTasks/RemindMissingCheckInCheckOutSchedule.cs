namespace NexTaskScheduler.AutoJobs.HRMTasks
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
    using NexTaskScheduler.Core.Models.HRM;
    using NexTaskScheduler.Criteria.HRM;
    using NexTaskScheduler.Service.Services.Interfaces;

    public class RemindMissingCheckInCheckOutSchedule : IJobSchedule
    {
        private readonly INexbotService nexbotService;
        private readonly IBaseService baseService;
        private readonly ILogService logService;
        private readonly IRecurringJobManager recurringJob;
        private readonly IOptionsSnapshot<CronExpressionConfigs> configs;

        public RemindMissingCheckInCheckOutSchedule(INexbotService nexbotService,
            IBaseService baseService,
            ILogService logService,
            IRecurringJobManager recurringJob,
            IOptionsSnapshot<CronExpressionConfigs> configs)
        {
            this.nexbotService = nexbotService;
            this.baseService = baseService;
            this.logService = logService;
            this.configs = configs;
            this.recurringJob = recurringJob;
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
            var missingCheckInOuts = await baseService.GetDataAsync<MissingCheckInOutEmployee>(
                new GetMissingCheckInCheckOutCriteria()
                ).ConfigureAwait(false);

            if (missingCheckInOuts.Any())
            {
                await SendMessageToGroups(categoryId, missingCheckInOuts);

                await SendMessageToEmployees(missingCheckInOuts.Where(e => !string.IsNullOrWhiteSpace(e.ConversationId)));
            }

            await logService.WriteCategoryMessageLog(categoryId);
        }

        private async Task SendMessageToGroups(MessageCategory categoryId, IEnumerable<MissingCheckInOutEmployee> missingCheckInOuts)
        {
            var conversationIds = await nexbotService.GetRegisteredConversation(categoryId);

            if (conversationIds.Any())
            {
                string msg = GenerateGroupMessage(missingCheckInOuts);

                foreach (var conversationId in conversationIds)
                {
                    await nexbotService.SendMessage(conversationId, msg);
                }
            }
        }

        private string GenerateGroupMessage(IEnumerable<MissingCheckInOutEmployee> missingCheckInOuts)
        {
            StringBuilder message = new StringBuilder();
            message.Append($"Hi All. There is/are {missingCheckInOuts.Count()} employee(s) got missing Check in/Check out: {Constants.NewLine}");

            foreach (var employee in missingCheckInOuts)
            {
                var missType = employee.Type == 1 ? "Check In" : "Check Out";
                message.Append($"_- {employee.FullName} ({employee.NickName}) missing {missType} on {employee.AttendanceDate.ToString("dd MMM, yyyy")}_ {Constants.NewLine}");
            }
            message.Append("Please access https://hrm.nexcel.vn/report/attendance to view more details. Thanks");

            return message.ToString();
        }

        private async Task SendMessageToEmployees(IEnumerable<MissingCheckInOutEmployee> employees)
        {
            foreach (var employee in employees)
            {
                var message = GenerateMessageForEmployee(employee);
                await nexbotService.SendMessage(employee.ConversationId, message);
            }
        }

        private static string GenerateMessageForEmployee(MissingCheckInOutEmployee employee)
        {
            var hrmLink = "https://hrm.nexcel.vn/checkinout/Create";
            var missType = employee.Type == 1 ? "Check In" : "Check Out";
            var message = $"Dear {employee.NickName}, {Constants.NewLine}";
            message += $"You have forgot _{missType}_ on _{employee.AttendanceDate.ToString("dd MMM, yyyy")}_ (shift {employee.ShiftCode}). {Constants.NewLine}";
            message += $"Please review and make a request at: {hrmLink}";
            return message;
        }
    }
}