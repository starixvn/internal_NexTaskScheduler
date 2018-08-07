namespace NexTaskScheduler.Criteria.BioStation
{
    using Fanex.Data.Repository;

    public class GetPendingBioDevicesCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Bio_GetPendingDevice";

        public override bool IsValid() => true;
    }
}