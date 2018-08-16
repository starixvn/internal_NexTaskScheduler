namespace NexTaskScheduler.Core.Models.Bio
{
    public class FingerprintAndCardSummary
    {
        public int EmpId { get; set; }

        public string FullName { get; set; }

        public string NickName { get; set; }

        public bool IsFingerprintWarning { get; set; }

        public bool IsCardWarning { get; set; }

        public bool IsWarning
        {
            get
            {
                return IsFingerprintWarning || IsCardWarning;
            }
        }
    }
}