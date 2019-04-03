namespace NexTaskScheduler.Service.Services
{
    using System;
    using System.Threading.Tasks;
    using NexTaskScheduler.Core.Enum;
    using NexTaskScheduler.Core.Extentions;
    using NexTaskScheduler.Criteria.Hangfire;
    using NexTaskScheduler.Service.Services.Interfaces;

    public class LogService : BaseService, ILogService
    {
        public async Task WriteLog(string messageType, string logContent)
        {
            var criteria = new WriteLogCriteria
            {
                MessageType = messageType,
                LogContent = logContent
            };

            await ExecuteNonQueryAsync(criteria);
        }

        public async Task WriteCategoryMessageLog(MessageCategory category)
        {
            string type = category.GetDisplayName();
            await WriteLog(type, $"Task {type} executed at {DateTime.Now.ToShortFormatedDate()}");
        }
    }
}