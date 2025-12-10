using StaffIo.Data.Enums;

namespace StaffIo.Data.Models
{
    /// <summary>
    /// Пользователь
    /// </summary>
    public class User
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string? MiddleName { get; set; }

        /// <summary>
        /// Фамилия пользователя
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Отчество пользователя
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// Дата создания пользователя
        /// </summary>
        public DateTime DateCreatrd { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Дата увольнения пользователя
        /// </summary>
        public DateTime? DateDeleted { get; set; }

        /// <summary>
        /// Статус пользователя
        /// </summary>
        public EnumUserStatus? Status { get; set; }

        /// <summary>
        /// Должность пользователя
        /// </summary>
        public string? Position { get; set; }

        /// <summary>
        /// Зарплата пользователя
        /// </summary>
        public decimal? Salary { get; set; }

        /// <summary>
        /// Работает ли пользователь сегодня
        /// </summary>
        public bool? ToDayWork { get; set; }

        /// <summary>
        /// Рабочий график пользователя
        /// </summary>
        public EnumWorkPlan? WorkPlan { get; set; }

        /// <summary>
        /// Разрешение на удвоолнение и принятие сотрудника
        /// </summary>
        public bool? AccessCanManage { get; set; }

        /// <summary>
        /// Роль пользователя
        /// </summary>
        public EnumUserRole TypeRole { get; set; }

        public Account Account { get; set; }

        public Session Session { get; set; }

        public List<User>? Deportament { get; set; }

        public Guid? OwnerId { get; set; }

        public User Owner { get; set; }

        public List<Foto> Fotos { get; set; }
    }
}
