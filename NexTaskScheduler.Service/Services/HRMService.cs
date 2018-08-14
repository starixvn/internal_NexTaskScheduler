namespace NexTaskScheduler.Service.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexTaskScheduler.Core.Models.HRM;
    using NexTaskScheduler.Criteria.HRM;
    using NexTaskScheduler.Service.Respositories.Interfaces;
    using NexTaskScheduler.Service.Services.Interfaces;

    public class HRMService : IHRMService
    {
        private readonly IHRMRepository hrmRepository;

        public HRMService(IHRMRepository hrmRepository)
        {
            this.hrmRepository = hrmRepository;
        }

        public async Task<IEnumerable<LateInEarlyOutDiscipline>> GetLateInEarlyOutDiscipline(GetViolateLateInEarlyOutDisciplineCriteria criteria)
        {
            IEnumerable<LateInEarlyOutDiscipline> lateInEarlyOuts = null;

            if (criteria != null && criteria.IsValid())
            {
                lateInEarlyOuts = await hrmRepository.GetLateInEarlyOutDiscipline(criteria);
            }

            return lateInEarlyOuts;
        }
    }
}