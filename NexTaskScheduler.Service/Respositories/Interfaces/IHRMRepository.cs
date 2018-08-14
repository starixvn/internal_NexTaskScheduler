namespace NexTaskScheduler.Service.Respositories.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using NexTaskScheduler.Core.Models.HRM;

    public interface IHRMRepository
    {
        Task<IEnumerable<LateInEarlyOutDiscipline>> GetLateInEarlyOutDiscipline(ICriteria criteria);
    }
}