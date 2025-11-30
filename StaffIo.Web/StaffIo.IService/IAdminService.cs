using StaffIo.IService.Models.AdminService.Request;
using StaffIo.IService.Models.AdminService.Response;

namespace StaffIo.IService
{
    public interface IAdminService
    {
        /// <summary>
        /// Удалить пользователя
        /// </summary>
        /// <returns></returns>
        Task<bool> Delete(AdminDeleteRequest request,Guid currentUserId);

        /// <summary>
        /// Добавить сотрудника
        /// </summary>
        /// <returns></returns>
        Task<bool> AddEmployee(AdminAddEmployeeRequest request, Guid currentUserId);

        /// <summary>
        /// Обновление прав доступа администратора на увольнение/принятия на работу сотрудников
        /// </summary>
        /// <returns></returns>
        Task<AdminAccessCanManageResponse> UpdateAccessCanManage(AdminAccessCanManageRequest request,Guid currentUserId);

        /// <summary>
        /// Обновить данные администратора/сотрудника
        /// </summary>
        /// <returns></returns>
        Task<AdminUpdateResponse> Update(AdminUpdateRequest request,Guid currentUserId);

        /// <summary>
        /// Обновить данные владельца
        /// </summary>
        /// <returns></returns>
        Task<AdminUpdateOwnerResponse> UpdateOwner(AdminUpdateOwnerRequest request, Guid currentUserId);

        /// <summary>
        /// Переместить сотрудника в другую организацию
        /// </summary>
        /// <returns></returns>
        Task<bool> Move(AdminMoveRequest request, Guid currentUserId);
    }
}
