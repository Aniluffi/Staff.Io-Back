namespace StaffIo.Data.Models
{
    /// <summary>
    /// Сессия
    /// </summary>
    public class Session
    {
        /// <summary>
        /// Идентификатор сессии
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Токен сессии
        /// </summary>
        public string SessionToken { get; set; }

        public Guid UserId { get; set; }

        public User User { get; set; }
    }
}
