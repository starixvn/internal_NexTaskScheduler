namespace NexTaskScheduler.InitJobs.AutoJobs
{
    using System;
    using System.Threading.Tasks;
    using Hangfire;
    using Hangfire.Common;
    using NexTaskScheduler.AutoJobs.Interfaces;
    using NexTaskScheduler.Core.Enum;
    using NexTaskScheduler.Interfaces.AutoJobs;
    using NexTaskScheduler.Service.Services.Interfaces;

    public class AutoJobInitial : IAutoJobInitial
    {
        private readonly Func<MessageCategory, IJobSchedule> autoReminder;
        private readonly ILogService logService;
        private readonly IRecurringJobManager recurringJob;

        public AutoJobInitial(Func<MessageCategory, IJobSchedule> autoReminder,
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