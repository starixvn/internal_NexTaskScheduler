namespace NexTaskScheduler.AutoJobs
{
    using System;
    using System.Threading.Tasks;
    using Hangfire;
    using Hangfire.Common;
    using NexTaskScheduler.Core.Enum;
    using NexTaskScheduler.Service.Services.Interfaces;

    public class InitAutoJobs
    {
        private readonly Func<MessageCategory, IJobSchedule> autoReminder;
        private readonly ILogService logService;
        private readonly IRecurringJobManager recurringJob;

        public InitAutoJobs(Func<MessageCategory, IJobSchedule> autoReminder,
            INexbotService nexbotService,
            ILogService logService,
            IRecurringJobManager recurringJob)
        {
            this.autoReminder = autoReminder;
            this.logService = logService;
            this.recurringJob = recurringJob;
        }

        public void Initialize()
        {
            foreach (MessageCategory category in Enum.GetValues(typeof(MessageCategory)))
            {
                autoReminder(category).Initialize();
            }

            recurringJob.AddOrUpdate("Server log", Job.FromExpression(() => WriteLogToDetectServerStatus()), Cron.MinuteInterval(15));
        }

        public async Task WriteLogToDetectServerStatus()
        {
            await logService.WriteLog("Server Status Log", $"Server still online at {DateTime.Now.ToLongTimeString()}");
        }
    }
}