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

    public class ViolateDisciplineScheduleWarning : IAutoReminder
    {
        private readonly IHRMService hrmService;
        private readonly INexbotService nexbotService;
        private readonly IRecurringJobManager recurringJob;
        private readonly IHangFireService hangFireService;
        private readonly IOptionsSnapshot<CronExpressionConfigs> configs;

        public ViolateDisciplineScheduleWarning(
            IHRMService hrmService,
            INexbotService nexbotService,
            IHangFireService hangFireService,
            IRecurringJobManager recurringJob,
            IOptionsSnapshot<CronExpressionConfigs> configs)
        {
            this.hrmService = hrmService;
            this.nexbotService = nexbotService;
            this.hangFireService = hangFireService;
            this.configs = configs;
            this.recurringJob = recurringJob;
        }

        public void Initialize()
        {
            recurringJob.AddOrUpdate(MessageCategory.ViolateLateInEarlyOutDiscipline.GetDisplayName(),
                Job.FromExpression(() => SendMessages(MessageCategory.ViolateLateInEarlyOutDiscipline)),
                configs.Value.ViolateLateInEarlyOutDiscipline,
                TimeZoneInfo.Local);
        }

        public async Task SendMessages(MessageCategory categoryId)
        {
            var toDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
            var fromDate = toDate.AddYears(-1).AddDays(1);

            var criteria = new GetViolateLateInEarlyOutDisciplineCriteria
            {
                FromDate = fromDate,
                ToDate = toDate
            };

            var violaters = await hrmService.GetLateInEarlyOutDiscipline(criteria);

            if (violaters.Any())
            {
                bool isChangedObject = await hangFireService.ObjectIsChangedOrNotExists(categoryId, violaters);

                if (isChangedObject)
                {
                    await SendMessageToGroup(violaters, fromDate, toDate);

                    await hangFireService.UpdateJsonObject(categoryId, violaters);
                }
            }

            await nexbotService.WriteCategoryMessageLog(categoryId);
        }

        private async Task SendMessageToGroup(IEnumerable<LateInEarlyOutDiscipline> violaters, DateTime fromDate, DateTime toDate)
        {
            var conversations = await nexbotService.GetRegisteredConversation(MessageCategory.ViolateLateInEarlyOutDiscipline);

            if (conversations.Any())
            {
                string message = GenerateGroupMessage(violaters, fromDate, toDate);

                foreach (var conversation in conversations)
                {
                    await nexbotService.SendMessage(conversation, message);
                }
            }
        }

        private string GenerateGroupMessage(IEnumerable<LateInEarlyOutDiscipline> violaters, DateTime fromDate, DateTime toDate)
        {
            StringBuilder message = new StringBuilder();
            message.Append($"Hi All. There is/are {violaters.Count()} employee(s) violate Late in/Early out discipline over 3 times from {fromDate.ToString("dd MMM, yyyy")} to {toDate.ToString("dd MMM, yyyy")}: {Constants.NewLine}");

            foreach (var violater in violaters)
            {
                message.Append($"_- {violater.FullName} ({violater.NickName}) violate {violater.TotalTimes} time(s)._ {Constants.NewLine}");
            }
            message.Append("Please access https://hrm.nexcel.vn/report/lateinearlyoutdiscipline to view more details. And don't forget to warning employees who violate the discipline over 3 times. Thanks");

            return message.ToString();
        }
    }
}