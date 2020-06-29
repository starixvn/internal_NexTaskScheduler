namespace NexTaskScheduler.Core.Models
{
    public class CronExpressionConfigs
    {
        public string BioDeviceSchedule { get; set; } = "0 0/12 * * 1-5";

        public string BioTimeChangeSchedule { get; set; } = "0 0/12 * * 1-5";

        public string FingerprintAndCardCheckSchedule { get; set; }

        public string MissingCheckInCheckOutSchedule { get; set; }

        public string CurrentMonthLateInEarlyOutSchedule { get; set; }

        public string LateInEarlyOutDeductSchedule { get; set; }

        public string ViolateLateInEarlyOutDiscipline { get; set; }

        public string RemindForgotLogtimeSchedule { get; set; }

        public string NexGoalAutoJobSchedule { get; set; } = "0 7 * * *";

        public string ScheduledMessageSchedule { get; set; }

        public string ReachedWorkingTimeSchedule { get; set; }

        public string ReSendErrorMessageSchedule { get; set; } = "*/1 * * * *";

        public string ReminderBirthdayAndFirstWorkingday { get; set; }
    }
}