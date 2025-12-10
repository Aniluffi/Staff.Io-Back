using Client.Files.IService;
using Client.Files.IService.Models.Request;
using Client.Files.IService.Models.Response;
using Client.Files.Options;
using Http.Client.Common;
using Http.Client.IService;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text.Json;

namespace Client.Files.Service
{
    /// <summary>
    /// сервис для работы с файлами googlrDisk
    /// </summary>
    public class FileB2Service : IFileB2Service
    {
        private IHttpClient _httpClient;
        private IBackblazeAuthService _backblazeAuthService;
        private BackBazeB2Options _options;

        public FileB2Service(IBackblazeAuthService backblazeAuthService, IOptions<BackBazeB2Options> options)
        {
            _options = options.Value;
            _backblazeAuthService = backblazeAuthService;
        }

        /// <summary>
        /// https://www.backblaze.com/apidocs/b2-delete-file-version
        /// </summary>
        /// <returns></returns>
        public async Task<BaseResponse<DeleteFileVersionResponse>> DeleteFileVersion(DeleteFileVersionRequest request)
        {
            var auth = await _backblazeAuthService.GetAuthAsync();

            _httpClient = new Http.Client.Service.HttpClient(auth.apiInfo.storageApi.apiUrl, new Http.Client.Models.Header
            {
                TypeAuth = "",
                AccessToken = auth.authorizationToken,
            });

            var response = await _httpClient.SendAsync<DeleteFileVersionResponse, DeleteFileVersionRequest>("/b2api/v4/b2_delete_file_version", HttpMethod.Post, request);

            return response;
        }

        /// <summary>
        /// https://www.backblaze.com/apidocs/b2-upload-file
        /// </summary>
        /// <returns></returns>
        public async Task<BaseResponse<UploadFileResponse>> UploadFile(UploadFileRequest request)
        {
            try
            {
                var uploadUrl = await GetUploadUrl(new GetUploadUrlRequest
                {
                    bucketId = _options.BasketId
                });

                if (!uploadUrl.IsSusses)
                {
                    throw new Exception(uploadUrl.ErrorMessage);
                }

                var fileBytes = Convert.FromBase64String(request.base64);

                var sha1 = ComputeSha1Hex(fileBytes);

                using var httpClient = new HttpClient();
                using var requestHttp = new HttpRequestMessage(HttpMethod.Post, uploadUrl.Data!.uploadUrl);

                // Авторизация
                requestHttp.Headers.TryAddWithoutValidation("Authorization", uploadUrl.Data.authorizationToken);

                // Заголовки B2
                requestHttp.Headers.Add("X-Bz-File-Name", Uri.EscapeDataString(request.fileName));
                requestHttp.Headers.Add("X-Bz-Content-Sha1", sha1);

                // Контент
                requestHttp.Content = new ByteArrayContent(fileBytes);
                requestHttp.Content.Headers.ContentType = new MediaTypeHeaderValue("b2/x-auto");
                requestHttp.Content.Headers.ContentLength = fileBytes.Length;

                var response = await httpClient.SendAsync(requestHttp);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var responseHttp = JsonSerializer.Deserialize<UploadFileResponse>(json)!;

                return new BaseResponse<UploadFileResponse>(responseHttp);
            }
            catch (Exception ex)
            {
                return new BaseResponse<UploadFileResponse>(ex.Message);
            }
        }

        /// <summary>
        /// https://www.backblaze.com/apidocs/b2-get-upload-url
        /// </summary>
        /// <returns></returns>
        private async Task<BaseResponse<GetUploadUrlResponse>> GetUploadUrl(GetUploadUrlRequest request)
        {
            try
            {
                var auth = await _backblazeAuthService.GetAuthAsync();

                _httpClient = new Http.Client.Service.HttpClient(auth.apiInfo.storageApi.apiUrl, new Http.Client.Models.Header
                {
                    AccessToken = auth.authorizationToken,
                });

                var response = await _httpClient.SendAsync<GetUploadUrlResponse, GetUploadUrlRequest>("/b2api/v4/b2_get_upload_url", HttpMethod.Post, request);

                return response;
            }
            catch (Exception ex)
            {
                return new BaseResponse<GetUploadUrlResponse>(ex.Message);
            }
        }

        private static string ComputeSha1Hex(byte[] data)
        {
            using var sha1 = SHA1.Create();
            var hash = sha1.ComputeHash(data);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}
