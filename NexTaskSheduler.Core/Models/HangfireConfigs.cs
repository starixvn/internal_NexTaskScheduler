namespace NexTaskScheduler.Core.Models
{
    public class HangfireConfigs
    {
        public string BioStationCronExpression { get; set; } = "0 0/12 * * 1-5";

        public string HRMCronExpression { get; set; } = "0 0/12 * * 1-5";

        public string LateInEarlyOutDeduction { get; set; }

        public string LateInEarlyDailySchedule { get; set; }
    }
}