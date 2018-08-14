namespace NexTaskScheduler.Service.Services.Interfaces
{
    using System.Threading.Tasks;
    using NexTaskScheduler.Core.Enum;

    public interface IHangFireService
    {
        Task<bool> ObjectIsChangedOrNotExists(MessageCategory categoryId, object value);

        Task<string> GetJsonObject(MessageCategory categoryId);

        Task UpdateJsonObject(MessageCategory categoryId, object value);
    }
}