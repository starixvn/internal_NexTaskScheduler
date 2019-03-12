namespace NexTaskScheduler.Core.Enum
{
    using System.ComponentModel.DataAnnotations;

    public enum MessageCategory
    {
        [Display(Name = "New Finger Machine Alert")]
        BioDevice = 1,

        [Display(Name = "BIO Change Time Alert")]
        BioTimeChange = 2,

        [Display(Name = "New Fingerprint & Card Alert")]
        BioFingerprintAndCard = 3,

        [Display(Name = "Deduct AL when exceeding Late In/ Early Out Alert")]
        DeductLateInEarlyOut = 4,

        [Display(Name = "Reminder Forgot spend time")]
        ForgotSpendTime = 5,

        [Display(Name = "Reminder Missing Check In/Out")]
        MissingCheckInCheckOut = 6,

        [Display(Name = "Warning Violate Late In/ Early Out In a Month")]
        LateInEarlyOutViolateInMonth = 7,

        [Display(Name = "Warning Not Follow Late In/ Early Out Policy")]
        ViolateLateInEarlyOutDiscipline = 8,

        [Display(Name = "NexGoal Auto Job")]
        NexGoalAutoJobSchedule = 9,
    }
}