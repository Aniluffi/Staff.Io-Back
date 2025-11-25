using StaffIo.IService.Models.AuthorizationService.Request;

namespace StaffIo.IService
{
    /// <summary>
    /// сервис для управления авторизацией пользователя
    /// </summary>
    public interface IAuthorizationService
    {
        /// <summary>
        /// метод для регистрации аккаунта
        /// </summary>
        /// <returns></returns>
        Task<string> Registration(AuthorizationRegistrationRequest request);

        /// <summary>
        /// метод для выхода из сессии
        /// </summary>
        /// <returns></returns>
        Task<bool> Logout(Guid currentUserId);

        /// <summary>
        /// метод для входа в сессию
        /// </summary>
        /// <returns></returns>
        Task<string> Login(AuthorizationLoginRequest request);

        /// <summary>
        /// метод ля получения текущего пользователя
        /// </summary>
        /// <returns></returns>
        Task<AuthorizationGetCurrentUser> GetCurrentUser(Guid Id);
    }
}
