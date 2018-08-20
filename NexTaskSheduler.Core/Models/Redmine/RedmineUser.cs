namespace NexTaskScheduler.Core.Models.Redmine
{
    using System;
    using System.Collections.Generic;

    public class RedmineUser
    {
        public int EmpId { get; set; }

        public string FullName { get; set; }

        public string NickName { get; set; }

        public int RedmineId { get; set; }

        public string ConversationId { get; set; }

        public IList<DateTime> ForgotLogtimeDates { get; set; }
    }
}