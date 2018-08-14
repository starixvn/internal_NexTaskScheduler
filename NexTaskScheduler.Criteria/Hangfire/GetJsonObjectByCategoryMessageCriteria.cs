namespace NexTaskScheduler.Criteria.Hangfire
{
    using Fanex.Data.Repository;
    using NexTaskScheduler.Core.Enum;

    public class GetJsonObjectByCategoryMessageCriteria : CriteriaBase
    {
        public MessageCategory CategoryId { get; set; }

        public override string GetSettingKey() => "NexHangfire_GetJsonObjectByCategoryId";

        public override bool IsValid() => CategoryId > 0;
    }
}