namespace NexTaskScheduler.Core.Models.HRM
{
    public class LateInEarlyOutViolateModel
    {
        public int EmpId { get; set; }

        public string FullName { get; set; }

        public string NickName { get; set; }

        public string ConversationId { get; set; }

        public int MinuteOnMonth { get; set; }

        public int TimeOnMonth { get; set; }
    }
}