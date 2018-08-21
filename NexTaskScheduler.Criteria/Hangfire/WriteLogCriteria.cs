namespace NexTaskScheduler.Criteria.Hangfire
{
    using Fanex.Data.Repository;

    public class WriteLogCriteria : CriteriaBase
    {
        public string MessageType { get; set; }

        public string LogContent { get; set; }

        public override string GetSettingKey() => "NexHangfire_WriteLog";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(MessageType)
                                       && !string.IsNullOrWhiteSpace(LogContent);
    }
}