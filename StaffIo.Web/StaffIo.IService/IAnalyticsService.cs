using StaffIo.IService.Models.AnalyticsService.Request;
using StaffIo.IService.Models.AnalyticsService.Resposne;

namespace StaffIo.IService
{
    public interface IAnalyticsService
    {
        /// <summary>
        /// Получение аналитики
        /// </summary>
        /// <returns></returns>S
        Task<AnalyticsGetResponse> Get(AnalyticsGetRequest request,Guid currentUserId);
    }
}
