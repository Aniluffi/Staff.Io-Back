using Microsoft.AspNetCore.Mvc;
using StaffIo.IService;
using StaffIo.IService.Models.HistoryService.Request;
using StaffIo.IService.Models.HistoryService.Response;
using StaffIo.Web.Common;

namespace StaffIo.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HistoryController : BaseRestController
    {
        private IHistoryService _historyService;

        public HistoryController(IHistoryService historyService)
        {
            _historyService = historyService;
        }

        /// <summary>
        /// получение истории для пользователя
        /// </summary>
        /// <returns></returns>
        [HttpGet("Get")]
        public async Task<HistoryGetResponse> Get([FromQuery]HistoryGetRequest request)
        {
            if (!UserId.HasValue)
                throw new Exception("Польхователь не авторизирован.");

            var response = await _historyService.Get(request,UserId.Value);
            return response;
        }
    }
}
