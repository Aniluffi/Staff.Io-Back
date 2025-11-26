using Microsoft.AspNetCore.Mvc;
using StaffIo.Data.Enums;
using StaffIo.IService;
using StaffIo.IService.Models.AuthorizationService.Request;
using StaffIo.Service;
using StaffIo.Web.Common;

namespace StaffIo.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthorizationController : BaseRestController
    {
        private readonly IAuthorizationService _authorizationService;

        public AuthorizationController(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        /// <summary>
        /// метод для регистрации аккаунта
        /// </summary>
        /// <returns></returns>
        [HttpPost("Registration")]
        public async Task<bool> Registration(AuthorizationRegistrationRequest request)
        {
            if(UserId.HasValue && request.UserRole == EnumUserRole.Owner)
                throw new Exception("Пользователь уже находится в сессии.");
            else if(!UserId.HasValue && request.UserRole == EnumUserRole.Admin)
                throw new Exception("Для регистрации пользователя с ролью Admin необходимо авторизоваться как пользователь с ролью Owner.");

            var registr = await _authorizationService.Registration(request,UserId);

            AddToken(registr);

            return true;
        }

        /// <summary>
        /// метод для выхода из сессии
        /// </summary>
        /// <returns></returns>
        [HttpDelete("Logout")]
        public async Task<bool> Logout()
        {
            if(!UserId.HasValue)
                throw new Exception("Пользователь не авторизован");

            var userId = UserId.HasValue ? UserId.Value : throw new Exception("Пользователь не авторизован");

            var logout = await _authorizationService.Logout(userId);

            RemoveToken();

            return logout;
        }

        /// <summary>
        /// метод для входа в сессию
        /// </summary>
        /// <returns></returns>
        [HttpPost("Login")]
        public async Task<bool> Login(AuthorizationLoginRequest request)
        {
            if(UserId.HasValue)
                throw new Exception("Пользователь уже находится в сессии.");

            var login = await _authorizationService.Login(request);

            AddToken(login);

            return true;
        }

        /// <summary>
        /// метод ля получения текущего пользователя
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetCurrentUser")]
        public async Task<AuthorizationGetCurrentUser> GetCurrentUser()
        {
            if(!UserId.HasValue)
                throw new Exception("Пользователь не авторизован");

            var userId = UserId.HasValue ? UserId.Value : throw new Exception("Пользователь не авторизован");

            var currentUser = await _authorizationService.GetCurrentUser(userId);
            return currentUser;
        }
    }
}
