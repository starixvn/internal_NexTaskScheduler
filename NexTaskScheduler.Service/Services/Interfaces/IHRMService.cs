namespace NexTaskScheduler.Service.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexTaskScheduler.Core.Models.HRM;
    using NexTaskScheduler.Criteria.HRM;

    public interface IHRMService
    {
        Task<IEnumerable<LateInEarlyOutDiscipline>> GetLateInEarlyOutDiscipline(GetViolateLateInEarlyOutDisciplineCriteria criteria);
    }
}