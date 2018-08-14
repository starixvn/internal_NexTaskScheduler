namespace NexTaskScheduler.Core.Models.HRM
{
    using System.Collections.Generic;

    public class LateInEarlyOutDiscipline
    {
        public int EmpId { get; set; }

        public string FullName { get; set; }

        public string NickName { get; set; }

        public string ConversationId { get; set; }

        public int TotalTimes { get; set; }

        public IEnumerable<LateInEarlyOutDisciplineDetail> Details { get; set; }
    }
}