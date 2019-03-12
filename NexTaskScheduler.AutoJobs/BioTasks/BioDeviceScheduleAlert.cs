namespace NexTaskScheduler.AutoJobs.BioTasks
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
    using NexTaskScheduler.Criteria.BioStation;
    using NexTaskScheduler.Service.Services.Interfaces;

    public class BioDeviceScheduleAlert : IJobSchedule
    {
        private readonly IBaseService baseService;
        private readonly INexbotService nexbotService;
        private readonly IRecurringJobManager recurringJob;
        private readonly IOptionsSnapshot<CronExpressionConfigs> configs;

        public BioDeviceScheduleAlert(IBaseService baseService,
            INexbotService nexbotService,
            IRecurringJobManager recurringJob,
            IOptionsSnapshot<CronExpressionConfigs> configs)
        {
            this.baseService = baseService;
            this.nexbotService = nexbotService;
            this.recurringJob = recurringJob;
            this.configs = configs;
        }

        public void Initialize()
        {
            recurringJob.AddOrUpdate(MessageCategory.BioDevice.GetDisplayName(),
                Job.FromExpression(() => RunJob(MessageCategory.BioDevice)),
                configs.Value.BioDeviceSchedule,
                TimeZoneInfo.Local);
        }

        public async Task RunJob(MessageCategory categoryId)
        {
            var pendingDevices = await baseService.GetDataAsync<BioDevice>(new GetPendingBioDevicesCriteria()).ConfigureAwait(false);
            if (pendingDevices.Any())
            {
                var conversationIds = await nexbotService.GetRegisteredConversation(categoryId).ConfigureAwait(false);

                if (conversationIds.Any())
                {
                    var message = GenerateMessage(pendingDevices);
                    foreach (var conversationId in conversationIds)
                    {
                        await nexbotService.SendMessage(conversationId, message).ConfigureAwait(false);
                    }
                }
            }

            await nexbotService.WriteCategoryMessageLog(categoryId);
        }

        private static string GenerateMessage(IEnumerable<BioDevice> bioDevices)
        {
            StringBuilder message = new StringBuilder();
            message.Append($"Hi All, There is/are {bioDevices.Count()} device(s) injected into the BIO system: {Constants.NewLine}");
            foreach (var device in bioDevices)
            {
                message.Append($"_- Device Id: {device.DeviceId}. Device Name: {device.DeviceName }_ {Constants.NewLine}");
            }
            message.Append($"Please review this. Thanks");
            return message.ToString();
        }
    }
}