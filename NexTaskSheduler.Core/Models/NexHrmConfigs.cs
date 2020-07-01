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
    }
}