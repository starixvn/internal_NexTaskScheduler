namespace NexTaskScheduler.Service.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexTaskScheduler.Core.Models.Redmine;

    public interface IRedmineService
    {
        Task<IEnumerable<RedmineUser>> GetForgotLogtimeEmployees();
    }
}