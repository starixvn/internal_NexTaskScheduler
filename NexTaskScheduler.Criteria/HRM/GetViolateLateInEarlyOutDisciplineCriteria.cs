namespace NexTaskScheduler.Criteria.HRM
{
    using System;
    using Fanex.Data.Repository;

    public class GetViolateLateInEarlyOutDisciplineCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey() => "HRM_GetLateInEarlyOutDisciplineViolate";

        public override bool IsValid() => true;
    }
}