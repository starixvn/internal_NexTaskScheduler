namespace NexTaskScheduler.Core.Models.HRM
{
    using System;

    public class LateInEarlyOutDisciplineDetail
    {
        public int EmpId { get; set; }

        public DateTime Month { get; set; }

        public int TotalTimes { get; set; }
    }
}