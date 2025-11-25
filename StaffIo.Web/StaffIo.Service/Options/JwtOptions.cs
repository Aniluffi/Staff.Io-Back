using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffIo.Service.Options
{
    /// <summary>
    /// настройки токена
    /// </summary>
    public class JwtOptions
    {
        /// <summary>
        /// секретный ключ для генерации jwt
        /// </summary>
        public string SecretKey { get; set; }
        /// <summary>
        /// время жизни токена
        /// </summary>
        public int ExpiresHours { get; set; }
        /// <summary>
        /// название токена
        /// </summary>
        public string NameToken { get; set; }
    }
}
