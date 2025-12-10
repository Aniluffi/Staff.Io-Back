using Client.Files.IService;
using Client.Files.IService.Models.Request;
using Microsoft.EntityFrameworkCore;
using StaffIo.Data;
using StaffIo.Data.Enums;
using StaffIo.Data.Models;
using StaffIo.IService;

namespace StaffIo.Service
{
    public class FileB2InternalService : IFileB2InternalService
    {
        private DbContextOptions<DataContext> _options;
        private IFileB2Service _fileB2Service;

        // Разрешенные расширения для фотографий
        private readonly List<string> _allowedExtensions = new List<string> { ".jpg", ".jpeg", ".png", ".webp" };

        public FileB2InternalService(DbContextOptions<DataContext> options, IFileB2Service service)
        {
            _options = options;
            _fileB2Service = service;
        }

        /// <summary>
        /// загрузка/обновление
        /// </summary>
        /// <returns></returns>
        public async Task<string> Upload(UploadFileRequest fileRequest, Guid userId, EnumTypeFoto? typeFoto, Guid? fotoId)
        {
            ValidateFileExtension(fileRequest.fileName);

            await using var db = new DataContext(_options);

            Foto foto = null;

            if (fotoId.HasValue)
            {
                foto = await db.Fotos.Where(c => c.Id == fotoId.Value && c.UserId == userId).FirstOrDefaultAsync();

                if (foto == null)
                    throw new Exception("Фото не найдено.");

                fileRequest.fileName = foto.FotoUrl;

                var upload = await _fileB2Service.UploadFile(fileRequest);

                if (!upload.IsSusses)
                {
                    throw new Exception(upload.ErrorMessage);
                }

                foto.FotoId = upload.Data!.fileId;
            }
            else
            {
                if (!typeFoto.HasValue)
                {
                    throw new Exception("Для добавления фото нудно указать его тип.");
                }


                var fotoPatch = GenerateFilePath(userId, typeFoto.Value, fileRequest.fileName);

                fileRequest.fileName = fotoPatch;

                foto = new Foto
                {
                    UserId = userId,
                    TypeFoto = typeFoto.Value,
                    FotoUrl = fileRequest.fileName,
                };

                var upload = await _fileB2Service.UploadFile(fileRequest);

                if (!upload.IsSusses)
                {
                    throw new Exception(upload.ErrorMessage);
                }

                foto.FotoId = upload.Data!.fileId;

                await db.AddAsync(foto);
            }

            await db.SaveChangesAsync();

            return foto.FotoUrl;
        }

        /// <summary>
        /// удаление
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Delete(DeleteFileVersionRequest request, Guid fotoId)
        {
            await using var db = new DataContext(_options);

            var deleteFromCloud = await _fileB2Service.DeleteFileVersion(request);

            if (!deleteFromCloud.IsSusses)
                throw new Exception(deleteFromCloud.ErrorMessage);

            db.Remove(new Foto
            {
                Id = fotoId,
            });

            await db.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Генерация пути к файлу на основе типа фотографии и идентификатора пользователя.
        /// </summary>
        /// <returns>Полный путь к файлу в хранилище.</returns>
        private string GenerateFilePath(Guid userId, EnumTypeFoto typeFoto, string originalFileName)
        {
            string folderName = typeFoto switch
            {
                EnumTypeFoto.Profile => "Profile",
                EnumTypeFoto.Document => "Documents",
                _ => throw new ArgumentOutOfRangeException(nameof(typeFoto), "Неизвестный тип фотографии.")
            };

            // Формат: users/{номер пользователя}/{Folder}/{название фото}
            // Используется Path.GetFileName, чтобы гарантировать, что в originalFileName нет пути.
            return string.Format("users/{0}/{1}/{2}", userId, folderName, Path.GetFileName(originalFileName));
        }

        /// <summary>
        /// Проверка расширения файла.
        /// </summary>
        /// <returns>True, если расширение разрешено.</returns>
        private bool ValidateFileExtension(string fileName)
        {
            var extension = Path.GetExtension(fileName)?.ToLowerInvariant();

            if (string.IsNullOrEmpty(extension) || !_allowedExtensions.Contains(extension))
            {
                throw new Exception($"Недопустимое расширение файла: {extension}. Разрешены только: {string.Join(", ", _allowedExtensions)}.");
            }
            return true;
        }
    }
}
