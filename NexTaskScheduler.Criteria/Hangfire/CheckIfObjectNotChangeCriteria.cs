namespace NexTaskScheduler.Criteria.Hangfire
{
    using Fanex.Data.Repository;
    using NexTaskScheduler.Core.Enum;

    public class CheckIfObjectNotChangeCriteria : CriteriaBase
    {
        public MessageCategory CategoryId { get; set; }

        public string JsonObject { get; set; }

        public override string GetSettingKey() => "NexHangfire_CheckIfObjectNotChange";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(JsonObject);
    }
}