namespace NexTaskScheduler.Core.Models
{
    public class JobSetting
    {
        public string JobName { get; set; }

        public bool IsEnabled { get; set; }

        public int TimeInterval { get; set; }

        public bool IsRunAtStarted { get; set; }

        public string RunAtFixedTime { get; set; }

        public string BaseUrl { get; set; }

        public string RequestUrl { get; set; }

        public string Method { get; set; }

        public bool IsAuth { get; set; }

        public string AuthRequestUrl { get; set; }

        public bool IsRunWhenStartApp { get; set; }
    }
}
