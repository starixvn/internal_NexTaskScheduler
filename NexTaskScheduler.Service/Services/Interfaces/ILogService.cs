namespace NexTaskScheduler.Service.Services.Interfaces
{
    using System.Threading.Tasks;
    using NexTaskScheduler.Core.Enum;

    public interface ILogService
    {
        Task WriteLog(string messageType, string logContent);

        Task WriteCategoryMessageLog(MessageCategory category);
    }
}