using StaffIo.Data.Enums;

namespace StaffIo.Data.Models
{
    /// <summary>
    /// История
    /// </summary>
    public class History
    {
        /// <summary>
        /// Идентификатор истории
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Идентификатор пользователя, который создал запись
        /// </summary>
        public Guid CreatedUserId { get; set; }

        /// <summary>
        /// Идентификатор пользователя, к которому относится запись
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Тип истории
        /// </summary>
        public EnumTypeHistory Type { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    }
}
