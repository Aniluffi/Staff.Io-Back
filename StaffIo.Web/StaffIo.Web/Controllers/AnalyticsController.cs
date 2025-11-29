using Microsoft.AspNetCore.Mvc;
using StaffIo.IService;
using StaffIo.IService.Models.AnalyticsService.Request;
using StaffIo.IService.Models.AnalyticsService.Resposne;
using StaffIo.Web.Common;

namespace StaffIo.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AnalyticsController : BaseRestController
    {
        private readonly IAnalyticsService _analyticsService;

        public AnalyticsController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        /// <summary>
        /// Получение аналитики
        /// </summary>
        /// <returns></returns>
        [HttpGet("Get")]
        public async Task<AnalyticsGetResponse> Get([FromQuery] AnalyticsGetRequest request)
        {
            if (!UserId.HasValue)
            {
                throw new Exception("Пользователь не авторизован");
            }

            var analytics = await _analyticsService.Get(request, UserId.Value);
            return analytics;
        }
    }
}
