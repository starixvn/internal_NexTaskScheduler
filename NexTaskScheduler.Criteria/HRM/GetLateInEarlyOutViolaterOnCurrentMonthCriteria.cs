namespace NexTaskScheduler.Criteria.HRM
{
    using System;
    using Fanex.Data.Repository;

    public class GetLateInEarlyOutViolaterOnCurrentMonthCriteria : CriteriaBase
    {
        public int Month { get; set; }

        public int Year { get; set; }

        public override string GetSettingKey() => "HRM_GetLateInEarlyOutOnMonth";

        public override bool IsValid() => Year <= DateTime.Now.Year;
    }
}