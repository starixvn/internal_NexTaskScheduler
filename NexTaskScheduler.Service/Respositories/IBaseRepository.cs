namespace NexTaskScheduler.Service.Respositories
{
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using NexTaskScheduler.Service.Models;

    public interface IBaseRepository
    {
        Task<CommandResult> ExecuteNonQueryAsync(ICriteria criteria);
    }
}