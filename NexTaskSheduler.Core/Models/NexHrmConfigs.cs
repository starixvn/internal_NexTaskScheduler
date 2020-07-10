namespace NexTaskScheduler.Core.Models
{
    public class NexHrmConfigs
    {
        public string ApiUrl { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string AuthTokenRoute { get; set; }

        public string ScheduledMessageRoute { get; set; }

        public string ReachedWorkingTimeRoute { get; set; }

        public string ReminderBirthdayAndFirstWorkingdayRoute { get; set; }

        public string ReminderMissingCheckInCheckOutScheduleRoute { get; set; }

        public string LateInEarlyOutViolateOnMonthScheduleWarningRoute { get; set; }

        public string ViolateDisciplineScheduleWarningRoute { get; set; }

        public string LateInEarlyOutDeductScheduleRoute { get; set; }
        
        public string BioDevice { get; set; }

        public string BioTimeChange { get; set; }

        public string BioFingerprintAndCard { get; set; }

        public string ForgotSpendTime { get; set; }

    }
}