namespace NexTaskScheduler.Service.Services
{
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using NexTaskScheduler.Core.Enum;
    using NexTaskScheduler.Criteria.Hangfire;
    using NexTaskScheduler.Service.Services.Interfaces;

    public class HangFireService : BaseService, IHangFireService
    {
        public async Task<bool> ObjectIsChangedOrNotExists(MessageCategory categoryId, object value)
        {
            string jsonObject = JsonConvert.SerializeObject(value);

            var criteria = new CheckIfObjectNotChangeCriteria
            {
                CategoryId = categoryId,
                JsonObject = jsonObject
            };

            return !await GetDataEntityAsync<bool>(criteria);
        }

        public async Task<string> GetJsonObject(MessageCategory categoryId)
        {
            var criteria = new GetJsonObjectByCategoryMessageCriteria
            {
                CategoryId = categoryId
            };

            return await GetDataEntityAsync<string>(criteria);
        }

        public async Task UpdateJsonObject(MessageCategory categoryId, object value)
        {
            string jsonObject = JsonConvert.SerializeObject(value);
            var criteria = new UpdateJsonObjectCriteria
            {
                CategoryId = categoryId,
                JsonObject = jsonObject
            };

            await ExecuteNonQueryAsync(criteria);
        }
    }
}