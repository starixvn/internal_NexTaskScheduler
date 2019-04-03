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
    using Newtonsoft.Json;
    using NexTaskScheduler.Core.Enum;
    using NexTaskScheduler.Core.Extentions;
    using NexTaskScheduler.Core.Models;
    using NexTaskScheduler.Core.Models.HRM;
    using NexTaskScheduler.Criteria.HRM;
    using NexTaskScheduler.Service.Services.Interfaces;

    public class LateInEarlyOutViolateOnMonthScheduleWarning : IJobSchedule
    {
        private readonly INexbotService nexbotService;
        private readonly IBaseService baseService;
        private readonly ILogService logService;
        private readonly IHangFireService hangFireService;
        private readonly IRecurringJobManager recurringJob;
        private readonly IOptionsSnapshot<CronExpressionConfigs> configs;

        public LateInEarlyOutViolateOnMonthScheduleWarning(
            INexbotService nexbotService,
            IBaseService baseService,
            ILogService logservice,
            IHangFireService hangFireService,
            IRecurringJobManager recurringJob,
            IOptionsSnapshot<CronExpressionConfigs> configs)
        {
            this.nexbotService = nexbotService;
            this.baseService = baseService;
            this.logService = logService;
            this.hangFireService = hangFireService;
            this.recurringJob = recurringJob;
            this.configs = configs;
        }

        public void Initialize()
        {
            recurringJob.AddOrUpdate(MessageCategory.LateInEarlyOutViolateInMonth.GetDisplayName(),
                Job.FromExpression(() => RunJob(MessageCategory.LateInEarlyOutViolateInMonth)),
                configs.Value.CurrentMonthLateInEarlyOutSchedule,
                TimeZoneInfo.Local);
        }

        public async Task RunJob(MessageCategory categoryId)
        {
            var criteria = new GetLateInEarlyOutViolaterOnCurrentMonthCriteria
            {
                Month = DateTime.Now.Day == 1 ? (DateTime.Now.Month - 1) : DateTime.Now.Month,
                Year = DateTime.Now.Year
            };
            var violaters = await baseService.GetDataAsync<LateInEarlyOutViolateModel>(criteria);

            if (violaters.Any())
            {
                var isChange = await hangFireService.ObjectIsChangedOrNotExists(categoryId, violaters);

                if (isChange)
                {
                    await SendMessageToViolatedEmployees(categoryId, violaters.Where(v => !string.IsNullOrWhiteSpace(v.ConversationId)));

                    await SendMessageToRegisterConversation(violaters.Where(v => v.TimeOnMonth >= 4), categoryId);

                    await hangFireService.UpdateJsonObject(categoryId, violaters);
                }
            }

            await logService.WriteCategoryMessageLog(categoryId);
        }

        private async Task SendMessageToViolatedEmployees(MessageCategory categoryId, IEnumerable<LateInEarlyOutViolateModel> violaters)
        {
            if (violaters.Any())
            {
                string jsonObject = await hangFireService.GetJsonObject(categoryId);
                if (!string.IsNullOrWhiteSpace(jsonObject))
                {
                    IEnumerable<LateInEarlyOutViolateModel> oldViolaters = JsonConvert.DeserializeObject<IEnumerable<LateInEarlyOutViolateModel>>(jsonObject);
                    foreach (var violater in violaters)
                    {
                        if (oldViolaters.Any(o => o.EmpId == violater.EmpId && violater.TimeOnMonth != o.TimeOnMonth))
                        {
                            string message = GeneratePersonalMessage(violater);
                            await nexbotService.SendMessage(violater.ConversationId, message);
                        }
                    }
                }
                else
                {
                    foreach (var violater in violaters)
                    {
                        string message = GeneratePersonalMessage(violater);
                        await nexbotService.SendMessage(violater.ConversationId, message);
                    }
                }
            }
        }

        private string GeneratePersonalMessage(LateInEarlyOutViolateModel violater)
        {
            string msg = $"Hi {violater.NickName},{Constants.NewLine}";
            if (violater.TimeOnMonth >= 4)
            {
                msg += $"There is inform that you got {violater.TimeOnMonth} time(s) Late in/Early out on {DateTime.Now.ToString("MMM, yyyy")}.{Constants.NewLine}";
                msg += $"Please notice that. Thanks";
            }
            else
            {
                msg += $"You got {violater.TimeOnMonth} time(s) Late in/Early out on {DateTime.Now.ToString("MMM, yyyy")}. If you Late in/Early out one more time on this month, you will violate Nexcel's policy.{Constants.NewLine}Please notice that. Thanks";
            }

            return msg;
        }

        private async Task SendMessageToRegisterConversation(IEnumerable<LateInEarlyOutViolateModel> violaters, MessageCategory categoryId)
        {
            var conversations = await nexbotService.GetRegisteredConversation(categoryId);
            if (conversations.Any())
            {
                string message = GenerateGroupMessage(violaters);
                foreach (var conversation in conversations)
                {
                    await nexbotService.SendMessage(conversation, message);
                }
            }
        }

        private string GenerateGroupMessage(IEnumerable<LateInEarlyOutViolateModel> violaters)
        {
            StringBuilder message = new StringBuilder();
            message.Append($"Hi All. There is/are {violaters.Count()} employee(s) Late in/Early out over 4 times on {DateTime.Now.ToString("MMM, yyyy")}: {Constants.NewLine}");

            foreach (var violater in violaters)
            {
                message.Append($"_- {violater.FullName} ({violater.NickName}) violate {violater.TimeOnMonth} time(s)._ {Constants.NewLine}");
            }
            message.Append("Please access https://hrm.nexcel.vn/report/lateinearlyout to view more details. And don't forget to warning employees who violate more than 4 times on this month. Thanks");

            return message.ToString();
        }
    }
}