namespace StaffIo.IService.Models.AdminService.Request
{
    // Запрос на увольнение/удаление сотрудника
    public class AdminDeleteRequest
    {
        // ID пользователя, которого нужно удалить/уволить
        public Guid UserId { get; set; }
    }
}
