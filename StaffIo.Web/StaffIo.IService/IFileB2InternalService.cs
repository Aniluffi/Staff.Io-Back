using Client.Files.IService.Models.Request;
using Client.Files.IService.Models.Response;
using StaffIo.Data.Enums;

namespace StaffIo.IService
{
    public interface IFileB2InternalService
    {
        /// <summary>
        /// загрузка/обновление
        /// </summary>
        /// <returns></returns>
        Task<string> Upload(UploadFileRequest fileRequest, Guid userId, EnumTypeFoto? typeFoto, Guid? fotoId);

        /// <summary>
        /// удаление
        /// </summary>
        /// <returns></returns>
        Task<bool> Delete(DeleteFileVersionRequest request,Guid fotoId);
    }
}
