using StaffIo.IService.Models.HistoryService.Request;
using StaffIo.IService.Models.HistoryService.Response;

namespace StaffIo.IService
{
    public interface IHistoryService
    {
        /// <summary>
        /// получение истории для пользователя
        /// </summary>
        /// <returns></returns>
        Task<HistoryGetResponse> Get(HistoryGetRequest request,Guid currentUserId);
    }
}
