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

        public bool Warning
        {
            get
            {
                if (PreviousLogTime <= TimeChange && TimeChange <= NextLogTime)
                {
                    return false;
                }

                if (Math.Abs((TimeChange - PreviousLogTime).Minutes) <= 2 || Math.Abs((TimeChange - NextLogTime).Minutes) <= 2)
                {
                    return false;
                }

                return true;
            }
        }
    }
}