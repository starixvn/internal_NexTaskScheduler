namespace NexTaskScheduler.Criteria.BioStation
{
    using System;
    using Fanex.Data.Repository;

    public class GetWarningBioTimeChangeCriteria : CriteriaBase
    {
        public DateTime OnDate { get; set; } = DateTime.Now;

        public override string GetSettingKey() => "Bio_GetWarningTimeChange";

        public override bool IsValid() => true;
    }
}