namespace NexTaskScheduler.AutoJobs
{
    using System.Threading.Tasks;
    using NexTaskScheduler.Core.Enum;

    public interface IAutoReminder
    {
        void Initialize();

        Task SendMessages(MessageCategory categoryId);
    }
}