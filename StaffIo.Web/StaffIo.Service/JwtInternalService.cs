using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StaffIo.IService;
using StaffIo.Service.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StaffIo.Service
{
    /// <summary>
    /// сервис для работы с Jwt
    /// </summary>
    public class JwtInternalService : IJwtInternalService
    {
        private JwtOptions _jwtOptions;

        public JwtInternalService(IOptions<JwtOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
        }

        /// <summary>
        /// метод для генерации Jwt токена
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public string GenerateToken(Guid userId)
        {
            Claim[] claims = [new("UserId", userId.ToString())];

            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey)), SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(signingCredentials: signingCredentials, expires: DateTime.UtcNow.AddHours(_jwtOptions.ExpiresHours), claims: claims);

            var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenValue;
        }

        /// <summary>
        /// метод для получения токена Jwt
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public string GetNameToken()
        {
            return _jwtOptions.NameToken;
        }

        /// <summary>
        /// метод для получения времени жизни Jwt(в часах)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public double GetTimeInHoursLiveJwt()
        {
            return _jwtOptions.ExpiresHours;
        }
    }
}
