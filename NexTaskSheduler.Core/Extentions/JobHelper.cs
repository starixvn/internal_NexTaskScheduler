namespace NexTaskScheduler.Core.Extentions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NexTaskScheduler.Core.Models;

    public static class JobHelper
    {
        public static JobSetting GetJobConfig(IEnumerable<JobSetting> jobConfigs, string jobName)
        {
            return jobConfigs.FirstOrDefault(job => job.JobName.Equals(jobName, StringComparison.InvariantCultureIgnoreCase));
        }

        public static bool IsEnabledJob(IEnumerable<JobSetting> jobConfigs, string jobName)
        {
            var settings = GetJobConfig(jobConfigs, jobName);

            return settings != null && settings.IsEnabled;
        }
    }
}
