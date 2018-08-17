namespace NexTaskScheduler.AutoJobs.HRMTasks
{
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
    using NexTaskScheduler.Core.Models.HRM;
    using NexTaskScheduler.Criteria.HRM;
    using NexTaskScheduler.Service.Services.Interfaces;

    public class RemindMissingCheckInCheckOutSchedule : IAutoReminder
    {
        private readonly INexbotService nexbotService;
        private readonly IBaseService baseService;
        private readonly IRecurringJobManager recurringJob;
        private readonly IOptionsSnapshot<CronExpressionConfigs> configs;

        public RemindMissingCheckInCheckOutSchedule(INexbotService nexbotService,
            IBaseService baseService,
            IRecurringJobManager recurringJob,
            IOptionsSnapshot<CronExpressionConfigs> configs)
        {
            this.nexbotService = nexbotService;
            this.baseService = baseService;
            this.configs = configs;
            this.recurringJob = recurringJob;
        }

        public void Initialize()
        {
            recurringJob.AddOrUpdate(MessageCategory.MissingCheckInCheckOut.GetDisplayName(),
                Job.FromExpression(() => SendMessages(MessageCategory.MissingCheckInCheckOut)),
                configs.Value.MissingCheckInCheckOutSchedule);
        }

        public async Task SendMessages(MessageCategory categoryId)
        {
            var missingCheckInOuts = await baseService.GetDataAsync<MissingCheckInOutEmployee>(
                new GetMissingCheckInCheckOutCriteria()
                ).ConfigureAwait(false);

            if (missingCheckInOuts.Any())
            {
                await SendMessageToGroups(categoryId, missingCheckInOuts);

                await SendMessageToEmployees(missingCheckInOuts.Where(e => !string.IsNullOrWhiteSpace(e.ConversationId)));
            }
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
            string message = $"Hi All. There is/are {missingCheckInOuts.Count()} employee(s) got missing Check in/Check out: {Constants.NewLine}";

            foreach (var employee in missingCheckInOuts)
            {
                var missType = employee.MissingType == 1 ? "Check In" : "Check Out";
                message += $"_- {employee.FullName} ({employee.NickName}) missing {missType} on {employee.AttendanceDate.ToShortFormatedDate()} _ {Constants.NewLine}";
            }
            message += "Please access https://hrm.nexcel.vn/report/attendance to view more details. Thanks ";

            return message;
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
            var missType = employee.MissingType == 1 ? "Check In" : "Check Out";
            var message = $"Dear {employee.NickName}, {Constants.NewLine}";
            message += $"You have forgot _{missType}_ on _{employee.AttendanceDate.ToString("dd, MMM, yyyy")}_ (shift {employee.ShiftCode}). {Constants.NewLine}";
            message += $"Please make a request at: {hrmLink}";
            return message;
        }
    }
}