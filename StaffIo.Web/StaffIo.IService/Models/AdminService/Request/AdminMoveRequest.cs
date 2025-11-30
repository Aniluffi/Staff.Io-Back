namespace StaffIo.IService.Models.AdminService.Request
{
    // Запрос на изменение руководителя/владельца для сотрудника (перемещение в иерархии)
    public class AdminMoveRequest
    {
        // ID пользователя, которого перемещают
        public Guid UserId { get; set; }

        // ID нового руководителя (null, если перемещают на верхний уровень)
        public Guid? OwnerId { get; set; }
    }
}
