namespace NexTaskScheduler.Core.Models
{
    public class BioDevice
    {
        public int DeviceId { get; set; }

        public string DeviceName { get; set; }

        public string DisplayName { get; set; }

        public string StatusId { get; set; }

        public string Status { get; set; }

        public bool IsActive { get; set; }
    }
}