namespace NexTaskScheduler.Criteria.Hangfire
{
    using Fanex.Data.Repository;
    using NexTaskScheduler.Core.Enum;

    public class UpdateJsonObjectCriteria : CriteriaBase
    {
        public MessageCategory CategoryId { get; set; }

        public string JsonObject { get; set; }

        public override string GetSettingKey() => "NexHangfire_UpdateJsonObject";

        public override bool IsValid() => CategoryId > 0;
    }
}