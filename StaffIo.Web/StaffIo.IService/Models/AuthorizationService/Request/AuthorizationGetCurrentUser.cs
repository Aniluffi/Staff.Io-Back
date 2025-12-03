using StaffIo.Data.Enums;

namespace StaffIo.IService.Models.AuthorizationService.Request
{
    /// <summary>
    /// модель для получения текущего пользователя
    /// </summary>
    public class AuthorizationGetCurrentUser
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }

        public string Login { get; set; }

        public string? FotoUrl { get; set; }

        public EnumUserRole Role { get; set; }
    }
}
