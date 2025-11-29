using Microsoft.AspNetCore.Mvc;
using StaffIo.IService;
using StaffIo.IService.Models.EmployeesServices.Request;
using StaffIo.IService.Models.EmployeesServices.Response;
using StaffIo.Web.Common;

namespace StaffIo.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeesController : BaseRestController
    {
        private IEmployeesService _employeesService;

        public EmployeesController(IEmployeesService employeesService)
        {
            _employeesService = employeesService;
        }

        /// <summary>
        /// Получить список сотрудников
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetList")]
        public async Task<EmployeesGetListResponse> GetList([FromQuery] EmployeesGetListRequest request)
        {
            if (!UserId.HasValue)
            {
                throw new Exception("Пользователь не авторизован");
            }

            var employees = await _employeesService.GetList(request, UserId.Value);
            return employees;
        }

        /// <summary>
        /// Получить детальный профиль пользователя
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetDetail")]
        public async Task<EmployeesGetDetailResponse> GetDetail([FromQuery] EmployeesGetDetailRequest request)
        {
            if (!UserId.HasValue)
            {
                throw new Exception("Пользователь не авторизован");
            }

            var employee = await _employeesService.GetDetail(request, UserId.Value);
            return employee;
        }

        /// <summary>
        /// Получить профиль текущего пользователя (кроме админа)
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetCurrentProfile")]
        public async Task<EmployeesGetCurrentProfileResponse> GetCurrentProfile()
        {
            if (!UserId.HasValue)
            {
                throw new Exception("Пользователь не авторизован");
            }

            var employee = await _employeesService.GetCurrentProfile(UserId.Value);
            return employee;
        }
    }
}
