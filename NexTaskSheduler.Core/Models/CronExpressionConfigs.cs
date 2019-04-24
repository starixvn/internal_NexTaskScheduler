namespace NexTaskScheduler.Core.Models
{
    public class CronExpressionConfigs
    {
        public string BioDeviceSchedule { get; set; }

        public string BioTimeChangeSchedule { get; set; }

        public string FingerprintAndCardCheckSchedule { get; set; }

        public string MissingCheckInCheckOutSchedule { get; set; }

        public string CurrentMonthLateInEarlyOutSchedule { get; set; }

        public string LateInEarlyOutDeductSchedule { get; set; }

        public string ViolateLateInEarlyOutDiscipline { get; set; }

        public string RemindForgotLogtimeSchedule { get; set; }

        public string NexGoalAutoJobSchedule { get; set; }

        public string ScheduledMessageSchedule { get; set; }
    }
}