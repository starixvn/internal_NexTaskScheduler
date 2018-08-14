namespace NexTaskScheduler.AutoJobs.HRMTasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Hangfire;
    using Hangfire.Common;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using NexTaskScheduler.Core.Constant;
    using NexTaskScheduler.Core.Enum;
    using NexTaskScheduler.Core.Extentions;
    using NexTaskScheduler.Core.Models;
    using NexTaskScheduler.Core.Models.HRM;
    using NexTaskScheduler.Criteria.HRM;
    using NexTaskScheduler.Service.Services.Interfaces;

    public class RemindViolateDisciplineSchedule : IAutoReminder
    {
        private readonly IHRMService hrmService;
        private readonly INexbotService nexbotService;
        private readonly IRecurringJobManager recurringJob;
        private readonly IHangFireService hangFireService;
        private readonly IOptionsSnapshot<HangfireConfigs> configs;
        private readonly DateTime fromDate = new DateTime(DateTime.Now.Year, 1, 1);
        private readonly DateTime toDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));

        public RemindViolateDisciplineSchedule(
            IHRMService hrmService,
            INexbotService nexbotService,
            IHangFireService hangFireService,
            IRecurringJobManager recurringJob,
            IOptionsSnapshot<HangfireConfigs> configs)
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
                configs.Value.LateInEarlyOutDeduction);
        }

        public async Task SendMessages(MessageCategory categoryId)
        {
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
                    await SendMessageToGroup(violaters);

                    await SendMessageToEmployees(categoryId, violaters.Where(v => !string.IsNullOrWhiteSpace(v.ConversationId)));

                    await hangFireService.UpdateJsonObject(categoryId, violaters);
                }
            }
        }

        private async Task SendMessageToEmployees(MessageCategory categoryId, IEnumerable<LateInEarlyOutDiscipline> violaters)
        {
            string jsonObject = await hangFireService.GetJsonObject(categoryId);

            if (!string.IsNullOrWhiteSpace(jsonObject))
            {
                IEnumerable<LateInEarlyOutDiscipline> oldViolaters = JsonConvert.DeserializeObject<IEnumerable<LateInEarlyOutDiscipline>>(jsonObject);

                foreach (var violater in violaters)
                {
                    if (oldViolaters.Any(o => o.EmpId == violater.EmpId && o.TotalTimes != violater.TotalTimes))
                    {
                        string message = GenerateEmployeeMessage(violater);
                        await nexbotService.SendMessage(violater.ConversationId, message);
                    }
                }
            }
        }

        private string GenerateEmployeeMessage(LateInEarlyOutDiscipline violater)
        {
            string msg = $"Hi {violater.NickName},{Constants.NewLine}";
            msg += $"There is inform that you has violate Late in/Early out discipline {violater.TotalTimes} time(s) form {fromDate.ToString("dd, MMM, yyyy")} to {toDate.ToString("dd, MMM, yyyy")}.{Constants.NewLine}";
            foreach (var detail in violater.Details)
            {
                msg += $"_- On {detail.Month.ToString("MM, yyyy")}. Total: {detail.TotalTimes} time(s)._{Constants.NewLine}";
            }
            msg += $"Please notice that. Thanks";
            return msg;
        }

        private async Task SendMessageToGroup(IEnumerable<LateInEarlyOutDiscipline> violaters)
        {
            var conversations = await nexbotService.GetRegisteredConversation(MessageCategory.ViolateLateInEarlyOutDiscipline);

            if (conversations.Any())
            {
                string message = GenerateGroupMessage(violaters);

                foreach (var conversation in conversations)
                {
                    await nexbotService.SendMessage(conversation, message);
                }
            }
        }

        private string GenerateGroupMessage(IEnumerable<LateInEarlyOutDiscipline> violaters)
        {
            string message = $"Hi All. There is/are {violaters.Count()} employee(s) violate Late in/Early out discipline from {fromDate.ToString("dd, MMM, yyyy")} to {toDate.ToString("dd, MMM, yyyy")}: {Constants.NewLine}";

            foreach (var violater in violaters)
            {
                string violateMonths = string.Join(",", violater.Details.Select(d => d.Month.ToString("MM, yyyy")));
                message += $"_- {violater.FullName} ({violater.NickName}) vioalte {violater.TotalTimes} time(s). On {violateMonths}._ {Constants.NewLine}";
            }
            message += "Please access http://localhost:51384/report/lateinearlyoutdiscipline to view more details. And don't forget to warning employees who violate the discipline. Thanks ";

            return message;
        }
    }
}