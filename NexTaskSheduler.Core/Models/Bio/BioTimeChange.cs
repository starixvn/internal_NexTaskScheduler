namespace NexTaskScheduler.Core.Models.Bio
{
    using System;

    public class BioTimeChange
    {
        public int DeviceId { get; set; }

        public string DeviceName { get; set; }

        public string DisplayName { get; set; }

        public DateTime TimeChange { get; set; }

        public DateTime PreviousLogTime { get; set; }

        public DateTime NextLogTime { get; set; }
    }
}