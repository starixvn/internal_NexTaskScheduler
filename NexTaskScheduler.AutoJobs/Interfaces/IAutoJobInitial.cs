namespace NexTaskScheduler.AutoJobs.Interfaces
{
    using System.Threading.Tasks;

    public interface IAutoJobInitial
    {
        void Initialize();

        Task WriteLogToDetectServerStatus();
    }
}