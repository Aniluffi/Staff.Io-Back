namespace StaffIo.IService.Models.AuthorizationService.Request
{
    /// <summary>
    /// запрос на авторизацию 
    /// </summary>
    public class AuthorizationLoginRequest
    {
        /// <summary>
        /// почта
        /// </summary>
        public string Login { get; set; }
        /// <summary>
        /// зашифрованыый пароль
        /// </summary>
        public string Password { get; set; }
    }
}
