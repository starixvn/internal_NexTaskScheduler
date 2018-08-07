namespace NexTaskScheduler.Criteria.HRM
{
    using Fanex.Data.Repository;

    public class GetMissingCheckInCheckOutCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "HRM_GetMissingCheckInCheckOut";

        public override bool IsValid() => true;
    }
}