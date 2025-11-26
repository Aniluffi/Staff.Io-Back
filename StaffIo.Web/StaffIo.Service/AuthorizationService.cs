using Microsoft.EntityFrameworkCore;
using StaffIo.Data;
using StaffIo.Data.Enums;
using StaffIo.Data.Models;
using StaffIo.IService;
using StaffIo.IService.Models.AuthorizationService.Request;
using StaffIo.Service.Common;

namespace StaffIo.Service
{
    /// <summary>
    /// сервис для авторизации
    /// </summary>
    public class AuthorizationService : IAuthorizationService
    {
        private DbContextOptions<DataContext> _options;
        private IJwtInternalService _jwtInternalService;

        public AuthorizationService(DbContextOptions<DataContext> options, IJwtInternalService jwtInternalService)
        {
            _jwtInternalService = jwtInternalService;
            _options = options;
        }

        /// <summary>
        /// метод для получения текушего пользователя
        /// </summary>
        /// <returns></returns>
        public async Task<AuthorizationGetCurrentUser> GetCurrentUser(Guid currentUserId)
        {
            await using var db = new DataContext(_options);

            var user = await db.Users
                .Where(u => u.Id == currentUserId)
                .Select(u => new AuthorizationGetCurrentUser
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    MiddleName = u.MiddleName,
                    LastName = u.LastName,
                    Login = u.Account.Login,
                }).FirstOrDefaultAsync();

            if (user == null)
                throw new Exception("Пользователь не найден");

            var getFotoUrl = await db.Fotos
                .Where(uf => uf.UserId == currentUserId)
                .Select(uf => uf.FotoUrl)
                .FirstOrDefaultAsync();

            user.FotoUrl = getFotoUrl;

            return user;
        }

        /// <summary>
        /// метод для входа в аккаунт
        /// </summary>
        /// <returns></returns>
        public async Task<string> Login(AuthorizationLoginRequest request)
        {
            await using var db = new DataContext(_options);

            var userAccount = await db.Accounts
                .FirstOrDefaultAsync(a => a.Login == request.Login && a.PasswordHash == request.Password.ToSha256Hash());

            if (userAccount == null)
                throw new Exception("Неверный логин или пароль");

            var sessionToken = _jwtInternalService.GenerateToken(userAccount.UserId);

            var getSession = await db.Sessions
                .FirstOrDefaultAsync(s => s.UserId == userAccount.UserId);

            if (getSession == null)
            {
                var newSession = new Session
                {
                    UserId = userAccount.UserId,
                    SessionToken = sessionToken,
                };

                await db.Sessions.AddAsync(newSession);
            }
            else
            {
                getSession.SessionToken = sessionToken;
            }

            await db.SaveChangesAsync();

            return sessionToken;
        }

        /// <summary>
        /// метод для выхода из аккаунта
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Logout(Guid currentUserId)
        {
            await using var db = new DataContext(_options);

            var getSession = await db.Sessions
                .FirstOrDefaultAsync(s => s.UserId == currentUserId);

            if (getSession == null)
                throw new Exception("Сессия не найдена");

            getSession.SessionToken = null;

            await db.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// метод для регистрации аккаунта
        /// </summary>
        /// <returns></returns>
        public async Task<string> Registration(AuthorizationRegistrationRequest model, Guid? currentUserId)
        {
            await using var db = new DataContext(_options);

            AuthorizationRegistrationRequest.Validation(model);

            var isHaveAccount = await db.Accounts
                .AnyAsync(a => a.Login == model.Login);

            if (isHaveAccount)
                throw new Exception("Аккаунт с таким логином уже существует");

            if (currentUserId != null)
            {
                var checkRoleOwner = await db.Users
                    .AnyAsync(u => u.Id == currentUserId && u.TypeRole == EnumUserRole.Owner);

                if (!checkRoleOwner)
                    throw new Exception("Для регистрации пользователя с ролью Admin необходимо авторизоваться как пользователь с ролью Owner.");
            }

            var userId = Guid.NewGuid();

            var sessionToken = _jwtInternalService.GenerateToken(userId);

            var newUser = new User
            {
                Id = userId,
                LastName = model.LastName,
                MiddleName = model.MiddleName,
                FirstName = model.FirstName,
                Account = new Account
                {
                    Login = model.Login,
                    PasswordHash = model.Password.ToSha256Hash(),
                },
                Session = new Session
                {
                    SessionToken = sessionToken,
                },
                TypeRole = model.UserRole
            };

            if (model.UserRole == EnumUserRole.Admin)
            {
                newUser.OwnerId = currentUserId;
                newUser.Status = EnumUserStatus.Active;
            }

            await db.Users.AddAsync(newUser);

            await db.SaveChangesAsync();

            return sessionToken;
        }
    }
}
