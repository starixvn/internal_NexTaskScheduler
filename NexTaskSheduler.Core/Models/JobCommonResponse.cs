namespace NexTaskScheduler.Core.Models
{
    public class JobCommonResponse
    {
        public int? Code { get; set; }

        public string Message { get; set; }

        public bool IsSuccessful { get; set; }
    }
}