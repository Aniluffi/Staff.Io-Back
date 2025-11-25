using Client.Files.IService.Models.Request;
using Client.Files.IService.Models.Response;
using Http.Client.Common;
using Http.Client.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Files.IService
{
    /// <summary>
    /// сервис для работы с файлами googlrDisk
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// метод для создания файла в googleDisk
        /// </summary>
        /// <returns></returns>
        Task<BaseResponse<CreateFileResponse>> CreateFile(CreateFileRequest request);
        /// <summary>
        /// метод для удаления файла в googleDisk
        /// </summary>
        /// <returns></returns>
        Task<BaseResponse<DeleteFileResponse>> DeleteFile(DeleteFileRequest request);
        /// <summary>
        /// метод для получения файла в googleDisk
        /// </summary>
        /// <returns></returns>
        Task<BaseResponse<GetFileResponse>> GetFile(GetFileRequest request);

        /// <summary>
        /// метод для обновления файла
        /// </summary>
        /// <returns></returns>
        Task<BaseResponse<UpdateFileResponse>> UpdateFile(UpdateFileRequest request);

        /// <summary>
        /// метод для копирования файла
        /// </summary>
        /// <returns></returns>
        Task<BaseResponse<CopyFileResponse>> CopyFile(CopyFileRequest request);
    }
}
