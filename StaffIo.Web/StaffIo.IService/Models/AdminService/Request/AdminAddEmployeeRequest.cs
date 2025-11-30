namespace StaffIo.IService.Models.AdminService.Request
{
    // Запрос на добавление нового сотрудника
    public class AdminAddEmployeeRequest
    {
        public Guid? UserId { get; set; }
        // Имя сотрудника
        public string FirstName { get; set; }

        // Отчество (опционально)
        public string? MiddleName { get; set; }

        // Фамилия (опционально)
        public string? LastName { get; set; }
    };
}
