using StaffIo.IService.Models.EmployeesServices.Request;
using StaffIo.IService.Models.EmployeesServices.Response;

namespace StaffIo.IService
{
    public interface IEmployeesService
    {
        /// <summary>
        /// Получить список сотрудников
        /// </summary>
        /// <returns></returns>
        Task<EmployeesGetListResponse> GetList(EmployeesGetListRequest request,Guid currentUserId);

        /// <summary>
        /// Получить детальный профиль пользователя
        /// </summary>
        /// <returns></returns>
        Task<EmployeesGetDetailResponse> GetDetail(EmployeesGetDetailRequest request,Guid currentUserId);

        /// <summary>
        /// Получить профиль текущего пользователя (кроме админа)
        /// </summary>
        /// <returns></returns>
        Task<EmployeesGetCurrentProfileResponse> GetCurrentProfile(Guid currentUserId);
    }
}
