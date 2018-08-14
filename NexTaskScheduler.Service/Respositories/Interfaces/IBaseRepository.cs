namespace NexTaskScheduler.Service.Respositories.Interfaces
{
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using NexTaskScheduler.Service.Models;

    public interface IBaseRepository
    {
        Task<CommandResult> ExecuteNonQueryAsync(ICriteria criteria);
    }
}