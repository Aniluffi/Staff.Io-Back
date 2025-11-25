using Microsoft.AspNetCore.Mvc;
using StaffIo.IService;
using System.IdentityModel.Tokens.Jwt;

namespace StaffIo.Web.Common
{
    public class BaseRestController : ControllerBase
    {

        private IJwtInternalService JWTService => HttpContext.RequestServices.GetRequiredService<IJwtInternalService>();
        private IHttpContextAccessor _httpContextAccessor => HttpContext.RequestServices.GetRequiredService<IHttpContextAccessor>();

        private string? _sessionToken
        {
            get
            {
                return _httpContextAccessor.HttpContext?.Request.Cookies[JWTService.GetNameToken()];
            }
        }

        public Guid? UserId
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_sessionToken))
                    return null;

                var jwt = new JwtSecurityTokenHandler().ReadJwtToken(_sessionToken);

                var getUserId = jwt.Claims.FirstOrDefault(c => c.Type == "UserId");

                if (getUserId == null || string.IsNullOrWhiteSpace(getUserId.Value))
                    return null;

                return new Guid(getUserId.Value);
            }
        }

        

        /// <summary>
        /// метод для добавления в куки токен
        /// </summary>
        /// <returns></returns>
        protected bool AddToken(string sessionToken)
        {
            HttpContext.Response.Cookies.Append(JWTService.GetNameToken(), sessionToken, new CookieOptions
            {
                HttpOnly = true,    // Защита от XSS
                Secure = true,      // Только HTTPS
                Expires = DateTime.UtcNow.AddHours(JWTService.GetTimeInHoursLiveJwt()),
                SameSite = SameSiteMode.Lax
            });

            return true;
        }

        protected bool RemoveToken()
        {
            HttpContext.Response.Cookies.Delete(JWTService.GetNameToken());
            return true;
        }
    }
}
