namespace NexTaskScheduler.Criteria.BioStation
{
    using System;
    using Fanex.Data.Repository;

    public class GetWarningBioTimeChangeCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey() => "Bio_GetWarningTimeChange";

        public override bool IsValid() => FromDate < ToDate;
    }
}