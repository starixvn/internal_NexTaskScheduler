namespace NexTaskScheduler.Service.Respositories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using NexTaskScheduler.Core.Models.HRM;
    using NexTaskScheduler.Service.Respositories.Interfaces;

    public class HRMRepository : IHRMRepository
    {
        public async Task<IEnumerable<LateInEarlyOutDiscipline>> GetLateInEarlyOutDiscipline(ICriteria criteria)
        {
            IEnumerable<LateInEarlyOutDiscipline> lateinEarlyOuts;

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var multi = await db.QueryMultipleAsync(criteria.GetSpParams()))
                {
                    lateinEarlyOuts = await multi.ReadAsync<LateInEarlyOutDiscipline>();

                    var lateInEarlyOutDetails = await multi.ReadAsync<LateInEarlyOutDisciplineDetail>();

                    foreach (var lateinEarlyOut in lateinEarlyOuts)
                    {
                        lateinEarlyOut.Details = lateInEarlyOutDetails.Where(e => e.EmpId == lateinEarlyOut.EmpId);
                    }
                }
            }

            return lateinEarlyOuts;
        }
    }
}