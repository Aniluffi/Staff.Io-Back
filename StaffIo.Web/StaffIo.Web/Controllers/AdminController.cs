using Microsoft.AspNetCore.Mvc;
using StaffIo.IService;
using StaffIo.IService.Models.AdminService.Request;
using StaffIo.IService.Models.AdminService.Response;
using StaffIo.Web.Common;

namespace StaffIo.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminController : BaseRestController
    {
        private IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        /// <summary>
        /// Удалить пользователя
        /// </summary>
        /// <returns></returns>
        [HttpDelete("Delete")]
        public async Task<bool> Delete(AdminDeleteRequest request)
        {
            if (!UserId.HasValue)
            {
                throw new Exception("Пользователь не авторизован");
            }
            var result = await _adminService.Delete(request, UserId.Value);
            return result;
        }

        /// <summary>
        /// Добавить сотрудника
        /// </summary>
        /// <returns></returns>
        [HttpPost("AddEmployee")]
        public async Task<bool> AddEmployee(AdminAddEmployeeRequest request)
        {
            if (!UserId.HasValue)
            {
                throw new Exception("Пользователь не авторизован");
            }

            var result = await _adminService.AddEmployee(request, UserId.Value);

            return result;
        }

        /// <summary>
        /// Обновление прав доступа администратора на увольнение/принятия на работу сотрудников
        /// </summary>
        /// <returns></returns>
        [HttpPatch("UpdateAccessCanManage")]
        public async Task<AdminAccessCanManageResponse> UpdateAccessCanManage(AdminAccessCanManageRequest request)
        {
            if (!UserId.HasValue)
            {
                throw new Exception("Пользователь не авторизован");
            }

            var result = await _adminService.UpdateAccessCanManage(request, UserId.Value);

            return result;
        }

        /// <summary>
        /// Обновить данные администратора/сотрудника
        /// </summary>
        /// <returns></returns>
        [HttpPatch("Update")]
        public async Task<AdminUpdateResponse> Update(AdminUpdateRequest request)
        {
            if (!UserId.HasValue)
            {
                throw new Exception("Пользователь не авторизован");
            }
            var result = await _adminService.Update(request, UserId.Value);
            return result;
        }

        /// <summary>
        /// Обновить данные владельца
        /// </summary>
        /// <returns></returns>
        [HttpPatch("UpdateOwner")]
        public async Task<AdminUpdateOwnerResponse> UpdateOwner(AdminUpdateOwnerRequest request)
        {
            if (!UserId.HasValue)
            {
                throw new Exception("Пользователь не авторизован");
            }
            var result = await _adminService.UpdateOwner(request, UserId.Value);
            return result;
        }

        /// <summary>
        /// Переместить сотрудника в другую организацию
        /// </summary>
        /// <returns></returns>
        [HttpPost("Move")]
        public async Task<bool> Move(AdminMoveRequest request)
        {
            if (!UserId.HasValue)
            {
                throw new Exception("Пользователь не авторизован");
            }
            var result = await _adminService.Move(request, UserId.Value);
            return result;
        }
    }
}
