using StaffIo.Data.Enums;

namespace StaffIo.IService.Models.EmployeesServices.Response
{
    public class EmployeesGetDetailResponse
    {
        // Уникальный идентификатор пользователя
        public Guid UserId { get; set; }

        // URL-адрес фотографии пользователя (нуль-допускающий)
        public string? UserFotoUrl { get; set; }

        // Статус пользователя (например, активен, уволен)
        // Тип перечисления, основанный на диаграмме
        public EnumUserStatus Status { get; set; }

        // Имя
        public string FirstName { get; set; } = string.Empty;

        // Фамилия
        public string LastName { get; set; } = string.Empty;

        // Отчество
        public string MiddleName { get; set; } = string.Empty;

        // Право на управление (нуль-допускающий)
        public bool? AccessCanManage { get; set; }

        // Список URL-адресов или идентификаторов документов
        public List<string> Documents { get; set; } = new List<string>();

        // Должность (нуль-допускающий)
        public string? Position { get; set; }

        // Роль пользователя (например, Администратор, Менеджер, Сотрудник)
        public EnumUserRole TypeRole { get; set; }

        // Оклад/Зарплата
        public decimal Salary { get; set; }

        public string? Login { get; set; }

        // Рабочий план (нуль-допускающий)
        // Тип перечисления, основанный на диаграмме
        public EnumWorkPlan? WorkPlan { get; set; }
    }
}
