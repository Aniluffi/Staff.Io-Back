using Microsoft.AspNetCore.Mvc;
using StaffIo.IService;
using StaffIo.IService.Models.ExpensesService.Response;
using StaffIo.Web.Common;

namespace StaffIo.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExpensesController : BaseRestController
    {
        private IExpensesService _expensesService;

        public ExpensesController(IExpensesService expensesService)
        {
            _expensesService = expensesService;
        }

        /// <summary>
        /// Получение расходов на зарплаты сотрудников
        /// </summary>
        /// <returns></returns>
        [HttpGet("Get")]
        public async Task<ExpensesGetResponse> Get()
        {
            var userId = UserId.HasValue ? UserId.Value : throw new Exception("Пользователь не авторизован");

            return await _expensesService.Get(userId);
        }
    }
}
