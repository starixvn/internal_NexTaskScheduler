namespace NexTaskScheduler.Service.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexTaskScheduler.Core.Enum;

    public interface INexbotService
    {
        Task SendMessage(string conversationId, string message);

        Task<IEnumerable<string>> GetRegisteredConversation(MessageCategory categoryId);

        Task WriteLog(string messageType, string logContent);

        Task WriteCategoryMessageLog(MessageCategory category);
    }
}