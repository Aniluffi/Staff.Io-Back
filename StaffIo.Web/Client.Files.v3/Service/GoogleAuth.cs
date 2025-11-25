using Client.Files.Options;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Http.Client.Models;
using Microsoft.Extensions.Options;

namespace Client.Files.Service
{
    /// <summary>
    /// класс для генерации токена авторизации
    /// </summary>
    public class GoogleAuth
    {
        public GoogleAuthOptions GoogleOptions { get; private set; }

        public GoogleAuth(IOptions<GoogleAuthOptions> googleAuthOptions)
        {
            GoogleOptions = googleAuthOptions.Value;
        }

        /// <summary>
        /// метод для получения Credential
        /// </summary>
        /// <returns></returns>
        private GoogleCredential CreateCredential()
        {
            var initializer = new ServiceAccountCredential.Initializer(GoogleOptions.ClientEmail)
            {
                Scopes = new[]
                {
                    DriveService.Scope.Drive,           // Полный доступ
                    DriveService.Scope.DriveFile,       // Доступ к файлам
                    DriveService.Scope.DriveAppdata,    // Доступ к app data
                    DriveService.Scope.DriveMetadata    // Доступ к метаданным
                }
            };

            var credential = new ServiceAccountCredential(initializer.FromPrivateKey(GoogleOptions.PrivateKey));
            return GoogleCredential.FromServiceAccountCredential(credential);
        }

        /// <summary>
        /// метод для получения токена авторизации
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetAccessToken()
        {
            var getCredential = CreateCredential();

            var getAccessToken = await getCredential.UnderlyingCredential.GetAccessTokenForRequestAsync();

            return getAccessToken;
        }

        /// <summary>
        /// метод для получения заголовка,что бы авторизировать при зпросе
        /// </summary>
        public async Task<Header> GetHeader()
        {
            var getAccessToken = await GetAccessToken();

            return new Header
            {
                TypeAuth = "Bearer",
                AccessToken = getAccessToken,
                MediaType = "application/octet-stream"
            };
        }
    }
}
