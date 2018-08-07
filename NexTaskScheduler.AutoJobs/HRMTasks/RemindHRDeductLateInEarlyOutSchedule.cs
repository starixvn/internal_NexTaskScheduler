namespace NexTaskScheduler.AutoJobs.HRMTasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using NexTaskScheduler.Core.Enum;
    using NexTaskScheduler.Core.Models;
    using NexTaskScheduler.Core.Models.HRM;
    using NexTaskScheduler.Criteria.HRM;
    using NexTaskScheduler.Service.Services.Interfaces;

    public class RemindHRDeductLateInEarlyOutSchedule : IAutoReminder
    {
        private readonly INexbotService nexbotService;
        private readonly IBaseService baseService;
        private readonly IOptionsSnapshot<HangfireConfigs> cron;

        public RemindHRDeductLateInEarlyOutSchedule(INexbotService nexbotService,
            IBaseService baseService,
            IOptionsSnapshot<HangfireConfigs> cron)
        {
            this.nexbotService = nexbotService;
            this.baseService = baseService;
            this.cron = cron;
        }

        public void Initialize()
        {
            throw new System.NotImplementedException();
        }

        public Task SendMessages(MessageCategory categoryId)
        {
            throw new System.NotImplementedException();
        }

        private bool IsWarningUserInCurrentMonth(LateInEarlyOutModel lateInEarlyOut)
        {
            string currentMonth = DateTime.Now.ToString("MMM");
            var violateTimeInCurrentMonth = lateInEarlyOut.GetType().GetProperty($"TimesIn{currentMonth}").GetValue(lateInEarlyOut, null).ToString();
            int time = int.Parse(violateTimeInCurrentMonth);
            return time > 3;
        }

        private async Task<IEnumerable<LateInEarlyOutModel>> GetDeductionBanlanceUsers()
        {
            var lateInEarlyOuts = await baseService.GetDataAsync<LateInEarlyOutModel>(new GetLateInEarlyOutDuductionCriteria());

            return lateInEarlyOuts.Where(e => e.BalanceCurrentYear >= 240);
        }
    }
}