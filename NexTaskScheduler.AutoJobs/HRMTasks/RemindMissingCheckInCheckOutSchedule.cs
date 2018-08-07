namespace NexTaskScheduler.AutoJobs.HRMTasks
{
    using System.Linq;
    using System.Threading.Tasks;
    using Hangfire;
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
        private readonly IOptionsSnapshot<HangfireConfigs> configs;

        public RemindMissingCheckInCheckOutSchedule(INexbotService nexbotService,
            IBaseService baseService,
            IOptionsSnapshot<HangfireConfigs> configs)
        {
            this.nexbotService = nexbotService;
            this.baseService = baseService;
            this.configs = configs;
        }

        public void Initialize()
        {
            RecurringJob.AddOrUpdate(MessageCategory.MissingCheckInCheckOut.GetDisplayName(),
                () => SendMessages(MessageCategory.MissingCheckInCheckOut),
                configs.Value.HRMCronExpression);
        }

        public async Task SendMessages(MessageCategory categoryId)
        {
            var missingCheckInOuts = await baseService.GetDataAsync<MissingCheckInOutEmployee>(
                new GetMissingCheckInCheckOutCriteria()
                ).ConfigureAwait(false);

            if (missingCheckInOuts.Any())
            {
                foreach (var emp in missingCheckInOuts)
                {
                    await SendMessageToEmployee(emp);
                }
            }
        }

        private async Task SendMessageToEmployee(MissingCheckInOutEmployee employee)
        {
            if (!string.IsNullOrWhiteSpace(employee.ConversationId))
            {
                var message = GenerateMessage(employee);
                await nexbotService.SendMessage(employee.ConversationId, message);
            }
        }

        private static string GenerateMessage(MissingCheckInOutEmployee employee)
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