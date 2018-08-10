namespace NexTaskScheduler.AutoJobs
{
    using System;
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
            foreach (MessageCategory category in Enum.GetValues(typeof(MessageCategory)))
            {
                if (category != MessageCategory.LateInEarlyOutWarningDeduct)
                {
                    autoReminder(category).Initialize();
                }
            }
        }
    }
}