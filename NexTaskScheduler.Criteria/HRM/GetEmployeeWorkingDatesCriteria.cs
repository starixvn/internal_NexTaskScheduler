namespace NexTaskScheduler.Criteria.HRM
{
    using System;
    using Fanex.Data.Repository;

    public class GetEmployeeWorkingDatesCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey() => "HRM_GetEmployeeWorkingDates";

        public override bool IsValid() => EmployeeId > 0 && FromDate < ToDate;
    }
}