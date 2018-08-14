namespace NexTaskScheduler.Criteria.Hangfire
{
    using Fanex.Data.Repository;
    using NexTaskScheduler.Core.Enum;

    public class GetRegisteredConversationCriteria : CriteriaBase
    {
        public MessageCategory CategoryId { get; set; }

        public override string GetSettingKey() => "Nexbot_GetRegisteredConversation";

        public override bool IsValid() => CategoryId > 0;
    }
}