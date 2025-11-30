using StaffIo.Data.Enums;
using StaffIo.IService.Models.EmployeesServices.Response;

namespace StaffIo.IService.Models.AdminService.Request
{
    public class AdminUpdateRequest 
    {
        // Уникальный идентификатор пользователя
        public Guid UserId { get; set; }

        // URL-адрес фотографии пользователя (нуль-допускающий)
        public string? UserFoto { get; set; }

        // Имя
        public string FirstName { get; set; } = string.Empty;

        // Фамилия
        public string LastName { get; set; } = string.Empty;

        // Отчество
        public string MiddleName { get; set; } = string.Empty;

        // Список URL-адресов или идентификаторов документов
        public List<string> Documents { get; set; } = new List<string>();

        // Должность (нуль-допускающий)
        public string? Position { get; set; }

        // Оклад/Зарплата
        public decimal Salary { get; set; }

        // Рабочий план (нуль-допускающий)
        // Тип перечисления, основанный на диаграмме
        public EnumWorkPlan? WorkPlan { get; set; }
    }
}
