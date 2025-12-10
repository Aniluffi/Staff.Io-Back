using Client.Files.IService.Models.Request;
using Client.Files.IService.Models.Response;
using Http.Client.Common;

namespace Client.Files.IService
{
    /// <summary>
    /// сервис для работы с файлами googlrDisk
    /// </summary>
    public interface IFileB2Service
    {
        /// <summary>
        /// https://www.backblaze.com/apidocs/b2-delete-file-version
        /// </summary>
        /// <returns></returns>
        Task<BaseResponse<DeleteFileVersionResponse>> DeleteFileVersion(DeleteFileVersionRequest request);

        /// <summary>
        /// https://www.backblaze.com/apidocs/b2-upload-file
        /// </summary>
        /// <returns></returns>
        Task<BaseResponse<UploadFileResponse>> UploadFile(UploadFileRequest request);
    }
}
