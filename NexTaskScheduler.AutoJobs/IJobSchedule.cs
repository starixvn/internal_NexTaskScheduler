namespace NexTaskScheduler.AutoJobs
{
    using System.Threading.Tasks;
    using NexTaskScheduler.Core.Enum;

    public interface IJobSchedule
    {
        void Initialize();

        Task RunJob(MessageCategory categoryId);
    }
}