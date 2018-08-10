namespace NexTaskScheduler.Criteria.HRM
{
    using Fanex.Data.Repository;

    public class GetLateInEarlyOutDuductionCriteria : CriteriaBase
    {
        public int Year { get; set; }

        public override string GetSettingKey() => "HRM_GetLateInEarlyOutToDeduct";

        public override bool IsValid() => Year > 2000;
    }
}