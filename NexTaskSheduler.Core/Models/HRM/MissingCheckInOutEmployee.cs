namespace NexTaskScheduler.Core.Models.HRM
{
    using System;

    public class MissingCheckInOutEmployee
    {
        public long Id { get; set; }

        public int EmpId { get; set; }

        public string FullName { get; set; }

        public string NickName { get; set; }

        public string ConversationId { get; set; }

        public string Team { get; set; }

        public string TeamCode { get; set; }

        public DateTime AttendanceDate { get; set; }

        public string ShiftCode { get; set; }

        public string Email { get; set; }

        public int MissingType { get; set; }
    }
}