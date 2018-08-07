namespace NexTaskScheduler.AutoJobs
{
    using System;
    using Hangfire;
    using Hangfire.Storage;
    using NexTaskScheduler.Core.Enum;

    public class InitAutoJobs
    {
        private readonly Func<MessageCategory, IAutoReminder> autoReminder;

        public InitAutoJobs(Func<MessageCategory, IAutoReminder> autoReminder)
        {
            this.autoReminder = autoReminder;
        }

        public void Initialize()
        {
            //RemoveAllRecurringJob();
            InitAllEvent();
        }

        private static void RemoveAllRecurringJob()
        {
            using (var connection = JobStorage.Current.GetConnection())
            {
                foreach (var recurringJob in connection.GetRecurringJobs())
                {
                    RecurringJob.RemoveIfExists(recurringJob.Id);
                }
            }
        }

        private void InitAllEvent()
        {
            autoReminder(MessageCategory.BioDevice).Initialize();
            autoReminder(MessageCategory.BioTimeChange).Initialize();
            autoReminder(MessageCategory.BioFingerprintAndCard).Initialize();
            autoReminder(MessageCategory.MissingCheckInCheckOut).Initialize();
            autoReminder(MessageCategory.ForgotSpendTime).Initialize();
        }
    }
}