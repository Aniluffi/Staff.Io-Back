using Client.Files.IService;
using Client.Files.IService.Models.Request;
using Client.Files.IService.Models.Response;
using Client.Files.Options;
using Http.Client.Common;
using Http.Client.IService;
using Microsoft.Extensions.Options;
using System.Management;

namespace Client.Files.Service
{
    /// <summary>
    /// сервис для работы с файлами googlrDisk
    /// </summary>
    public class FileService : IFileService
    {
        private IHttpClient _httpClient;
        private IHttpClient _httpClientUpload;
        private IOptions<GoogleAuthOptions> _options;

        public FileService(IOptions<GoogleAuthOptions> authOptions)
        {
            _options = authOptions;
        }

        /// <summary>
        /// метод для копирования файла
        /// </summary>
        /// <returns></returns>
        public async Task<BaseResponse<CopyFileResponse>> CopyFile(CopyFileRequest request)
        {
            await OnClient();

            var response = await _httpClient.SendAsync<CopyFileResponse, CopyFileRequest>(@$"/files/{request.Id}/copy", HttpMethod.Post, request);

            if (!response.IsSusses)
                return new BaseResponse<CopyFileResponse>(response.ErrorMessage ?? "");

            return response;
        }

        /// <summary>
        /// метод для создания файла в googleDisk
        /// </summary>
        /// <returns></returns>
        public async Task<BaseResponse<CreateFileResponse>> CreateFile(CreateFileRequest request)
        {
            await OnClient();

            var response = await _httpClientUpload.SendAsync<CreateFileResponse, object>(@"/files?uploadType=multipart", HttpMethod.Post, new
            {
                name = request.name ?? "" + Guid.NewGuid().ToString(),
                request.mimeType,
                parents = new List<string>
                {
                    _options.Value.BaseFolder
                }
            }, request.File);

            if (!response.IsSusses)
                return new BaseResponse<CreateFileResponse>(response.ErrorMessage ?? "");

            return response;
        }

        /// <summary>
        /// метод для удаления файла в googleDisk
        /// </summary>
        /// <returns></returns>
        public async Task<BaseResponse<DeleteFileResponse>> DeleteFile(DeleteFileRequest request)
        {
            await OnClient();

            var response = await _httpClient.SendAsync<DeleteFileResponse, DeleteFileRequest>(@$"/files/{request.id}", HttpMethod.Delete, request);

            if (!response.IsSusses)
                return new BaseResponse<DeleteFileResponse>(response.ErrorMessage ?? "");

            return response;
        }

        /// <summary>
        /// метод для получения файла в googleDisk
        /// </summary>
        /// <returns></returns>
        public async Task<BaseResponse<GetFileResponse>> GetFile(GetFileRequest request)
        {
            await OnClient();

            var response = await _httpClient.SendAsync<GetFileResponse, GetFileRequest>(@$"/files/{request.id}?fields=id,name,mimeType,webViewLink,webContentLink", HttpMethod.Get, request);

            if (!response.IsSusses)
                return new BaseResponse<GetFileResponse>(response.ErrorMessage ?? "");

            return response;
        }

        /// <summary>
        /// метод для обновления файла
        /// </summary>
        /// <returns></returns>
        public async Task<BaseResponse<UpdateFileResponse>> UpdateFile(UpdateFileRequest request)
        {
            await OnClient();

            if (request.File == null || request.File.Length == 0)
                return new BaseResponse<UpdateFileResponse>("Необходимо указать файл");

            var response = await _httpClientUpload.SendAsync<UpdateFileResponse, object>(@$"/files/{request.Id}?uploadType=multipart", HttpMethod.Patch, new { request.mimeType }, request.File);

            if (!response.IsSusses)
                return new BaseResponse<UpdateFileResponse>(response.ErrorMessage ?? "");

            return response;
        }

        /// <summary>
        /// включение клиента
        /// </summary>
        private async Task OnClient()
        {
            var header = await new GoogleAuth(_options).GetHeader();

            _httpClientUpload = new Http.Client.Service.HttpClient(_options.Value.BasePatchUpload, header);
            _httpClient = new Http.Client.Service.HttpClient(_options.Value.BasePatch, header);
        }
    }
}
