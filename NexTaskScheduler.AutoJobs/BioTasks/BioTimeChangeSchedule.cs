namespace NexTaskScheduler.AutoJobs.BioTasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Hangfire;
    using Microsoft.Extensions.Options;
    using NexTaskScheduler.Core.Constant;
    using NexTaskScheduler.Core.Enum;
    using NexTaskScheduler.Core.Extentions;
    using NexTaskScheduler.Core.Models;
    using NexTaskScheduler.Core.Models.Bio;
    using NexTaskScheduler.Criteria.BioStation;
    using NexTaskScheduler.Service.Services.Interfaces;

    public class BioTimeChangeSchedule : IAutoReminder
    {
        private readonly INexbotService nexbotService;
        private readonly IBaseService baseService;
        private readonly IRecurringJobManager recurringJob;
        private readonly IOptionsSnapshot<HangfireConfigs> configs;

        private readonly DateTime today = DateTime.Now;

        public BioTimeChangeSchedule(IBaseService baseService,
            INexbotService nexbotService,
            IRecurringJobManager recurringJob,
            IOptionsSnapshot<HangfireConfigs> configs)
        {
            this.baseService = baseService;
            this.nexbotService = nexbotService;
            this.recurringJob = recurringJob;
            this.configs = configs;
        }

        public void Initialize()
        {
            RecurringJob.AddOrUpdate(MessageCategory.BioTimeChange.GetDisplayName(),
                () => SendMessages(MessageCategory.BioTimeChange),
                configs.Value.BioStationCronExpression);
        }

        public async Task SendMessages(MessageCategory categoryId)
        {
            var warningTimeChanges = await baseService.GetDataAsync<BioTimeChange>(new GetWarningBioTimeChangeCriteria());
            warningTimeChanges = warningTimeChanges.Where(t => t.Warning);

            if (warningTimeChanges.Any())
            {
                var conversationIds = await nexbotService.GetRegisteredConversation(categoryId).ConfigureAwait(false);

                if (conversationIds.Any())
                {
                    var message = GenerateMesssage(warningTimeChanges);
                    foreach (var conversationId in conversationIds)
                    {
                        await nexbotService.SendMessage(conversationId, message).ConfigureAwait(false);
                    }
                }
            }
        }

        private string GenerateMesssage(IEnumerable<BioTimeChange> timeChanges)
        {
            var message = new StringBuilder();
            message.Append($"Hi All, There is/are {timeChanges.Count()} change(s) of time, today ({today.ToShortFormatedDate()}): {Constants.NewLine}");
            foreach (var timeChange in timeChanges)
            {
                message.Append($"_- Device Id: {timeChange.DeviceId}. Device Name: {timeChange.DeviceName}. Time Changed: {timeChange.TimeChange.ToShortFormatedDate()}. _ {Constants.NewLine}");
            }
            message.Append($"Please, review the change(s). Thanks");
            return message.ToString();
        }
    }
}