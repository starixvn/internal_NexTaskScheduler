namespace NexTaskScheduler.Criteria.HRM
{
    using Fanex.Data.Repository;

    public class GetLateInEarlyOutDuductionCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "HRM_GetLateInEarlyOutDeduction";

        public override bool IsValid() => true;
    }
}