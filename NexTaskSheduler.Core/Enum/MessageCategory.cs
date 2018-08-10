namespace NexTaskScheduler.Core.Enum
{
    using System.ComponentModel.DataAnnotations;

    public enum MessageCategory
    {
        [Display(Name = "New Finger Machine")]
        BioDevice = 1,

        [Display(Name = "BIO Change Time")]
        BioTimeChange = 2,

        [Display(Name = "Fingerprint & Card")]
        BioFingerprintAndCard = 3,

        [Display(Name = "Missing Check In/Check Out")]
        MissingCheckInCheckOut = 4,

        [Display(Name = "Forgot spend time Remind")]
        ForgotSpendTime = 5,

        [Display(Name = "Late in/Early out Deduction Warning")]
        LateInEarlyOutWarningDeduct = 6,
    }
}