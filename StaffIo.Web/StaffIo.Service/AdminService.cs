using Client.Files.IService;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StaffIo.Data;
using StaffIo.Data.Enums;
using StaffIo.Data.JsonModels;
using StaffIo.Data.Models;
using StaffIo.IService;
using StaffIo.IService.Models.AdminService.Request;
using StaffIo.IService.Models.AdminService.Response;
using StaffIo.Service.Extensions;

namespace StaffIo.Service
{
    public class AdminService : IAdminService
    {
        private DbContextOptions<DataContext> _options;
        private IFileB2InternalService _fileService;

        public AdminService(DbContextOptions<DataContext> options, IFileB2InternalService fileService)
        {
            _fileService = fileService;
            _options = options;
        }

        /// <summary>
        /// Удалить пользователя
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Delete(AdminDeleteRequest request, Guid currentUserId)
        {
            await using var db = new DataContext(_options);

            if (!await db.Users.AnyAsync(c => (c.Id == currentUserId && c.TypeRole == EnumUserRole.Admin && (c.AccessCanManage ?? false)) || c.TypeRole == EnumUserRole.Owner && c.Id == currentUserId))
            {
                throw new Exception("Нет доступа для уврльнения.");
            }

            // Проверка, что пользователь доступен текущему
            var getAccessUserIds = await GetAccessUserIds(db, new List<Guid> { currentUserId });
            getAccessUserIds.Add(currentUserId);

            var checkAccess = getAccessUserIds.Contains(request.UserId);

            if (!checkAccess)
                throw new Exception("Нет доступа для удаления данного пользователя");

            var user = await db.Users.Where(c => c.OwnerId.HasValue && c.Id != currentUserId && c.Id == request.UserId)
                .FirstOrDefaultAsync();

            if (user == null)
                throw new Exception("Пользователь не найден");

            if (user.TypeRole == Data.Enums.EnumUserRole.Employee)
            {
                user.DateDeleted = DateTime.UtcNow;
                user.Status = Data.Enums.EnumUserStatus.Deactivated;

                await AddHistory(db, EnumTypeHistory.Deleted, new JsonHistoryValue
                {
                    Value = null
                }, user.Id, currentUserId);
            }
            else
            {
                var getSubUsers = await db.Users
                    .Where(c => c.OwnerId.HasValue && c.OwnerId == request.UserId)
                    .ToListAsync();

                foreach (var subUser in getSubUsers)
                {
                    subUser.OwnerId = currentUserId;
                }

                db.Users.Remove(user);
            }

            await db.SaveChangesAsync();

            return true;
        }

        private async Task<List<Guid>> GetAccessUserIds(DataContext data, List<Guid> userIds)
        {
            var getUsersIds = await data.Users
                .Where(u => u.OwnerId.HasValue && userIds.Contains(u.OwnerId.Value))
                .Select(u => u.Id)
                .ToListAsync();

            if (getUsersIds.Count == 0)
                return getUsersIds;

            var getSubUsers = await GetAccessUserIds(data, getUsersIds);

            getUsersIds.AddRange(getSubUsers);

            return getUsersIds;
        }

        /// <summary>
        /// Добавить сотрудника
        /// </summary>
        /// <returns></returns>
        public async Task<bool> AddEmployee(AdminAddEmployeeRequest request, Guid currentUserId)
        {
            await using var db = new DataContext(_options);

            //проверка на права
            if (!await db.Users.AnyAsync(c => (c.Id == currentUserId && c.TypeRole == EnumUserRole.Admin && (c.AccessCanManage ?? false)) || c.TypeRole == EnumUserRole.Owner && c.Id == currentUserId))
            {
                throw new Exception("Нет доступа для принятия.");
            }

            if (!request.UserId.HasValue)
            {
                var userId = Guid.NewGuid();

                await db.Users.AddAsync(new Data.Models.User
                {
                    Id = userId,
                    OwnerId = currentUserId,
                    TypeRole = EnumUserRole.Employee,
                    Status = EnumUserStatus.Active,
                    DateCreatrd = DateTime.UtcNow,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    MiddleName = request.MiddleName,
                });

                await AddHistory(db, EnumTypeHistory.Add, new JsonHistoryValue
                {
                    Value = null
                }, userId, currentUserId);
            }
            else
            {
                // Проверка, что пользователь доступен текущему
                var getAccessUserIds = await GetAccessUserIds(db, new List<Guid> { currentUserId });
                getAccessUserIds.Add(currentUserId);

                var checkAccess = getAccessUserIds.Contains(request.UserId.Value);

                if (!checkAccess)
                    throw new Exception("Нет доступа для удаления данного пользователя");

                //проверка на права
                if (!await db.Users.AnyAsync(c => (c.Id == currentUserId && c.TypeRole == EnumUserRole.Admin && (c.AccessCanManage ?? false)) || c.TypeRole == EnumUserRole.Owner))
                {
                    throw new Exception("Нет доступа для увольнения.");
                }

                var getAccount = await db.Users
                .FirstOrDefaultAsync(c => c.Id == request.UserId.Value);

                if (getAccount == null)
                    throw new Exception("Пользователь не найден");

                getAccount.Status = EnumUserStatus.Active;
                getAccount.DateDeleted = null;

                await AddHistory(db, EnumTypeHistory.Add, new JsonHistoryValue
                {
                    Value = null
                }, getAccount.Id, currentUserId);
            }

            await db.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Обновление прав доступа администратора на увольнение/принятия на работу сотрудников
        /// </summary>
        /// <returns></returns>
        public async Task<AdminAccessCanManageResponse> UpdateAccessCanManage(AdminAccessCanManageRequest request, Guid currentUserId)
        {
            await using var db = new DataContext(_options);

            //проверка на права
            if (!await db.Users.AnyAsync(c => c.Id == currentUserId && c.TypeRole == EnumUserRole.Owner))
            {
                throw new Exception("Нет доступа для выдачи прав увольнения/принятия.");
            }

            var admin = await db.Users
                .FirstOrDefaultAsync(c => c.Id == request.UserId);

            if (admin == null)
            {
                throw new Exception("Пользователь не найден.");
            }

            if (admin.TypeRole != EnumUserRole.Admin)
            {
                throw new Exception("Пользователь не является администратором.");
            }

            admin.AccessCanManage = !admin.AccessCanManage;

            await AddHistory(db, EnumTypeHistory.ChengeAccessCanManage, new JsonHistoryValue
            {
                AccessCanManage = !admin.AccessCanManage,
            }, request.UserId, currentUserId);

            await db.SaveChangesAsync();

            return new AdminAccessCanManageResponse
            {
                CurrentAccess = admin.AccessCanManage!.Value,
            };
        }

        /// <summary>
        /// Обновить данные администратора/сотрудника
        /// </summary>
        /// <returns></returns>
        public async Task<AdminUpdateResponse> Update(AdminUpdateRequest request, Guid currentUserId)
        {
            await using var db = new DataContext(_options);

            // Проверка, что пользователь доступен текущему
            var getAccessUserIds = await GetAccessUserIds(db, new List<Guid> { currentUserId });
            getAccessUserIds.Add(currentUserId);

            var checkAccess = getAccessUserIds.Contains(request.UserId);

            if (!checkAccess)
                throw new Exception("Нет доступа к пользователю");

            var user = await db.Users
                .FirstOrDefaultAsync(c => c.Id == request.UserId);

            if (user == null)
                throw new Exception("Пользователь не найден");

            if (!string.IsNullOrWhiteSpace(request.FirstName) && user.FirstName != request.FirstName)
            {
                user.FirstName = request.FirstName;

                await AddHistory(db, EnumTypeHistory.ChangeFirstName, new JsonHistoryValue
                {
                    Value = request.FirstName,
                }, user.Id, currentUserId);
            }

            if (!string.IsNullOrWhiteSpace(request.LastName) && user.LastName != request.LastName)
            {
                user.LastName = request.LastName;

                await AddHistory(db, EnumTypeHistory.ChangeLastName, new JsonHistoryValue
                {
                    Value = request.LastName,
                }, user.Id, currentUserId);
            }

            if (!string.IsNullOrWhiteSpace(request.MiddleName) && user.MiddleName != request.MiddleName)
            {
                user.MiddleName = request.MiddleName;

                await AddHistory(db, EnumTypeHistory.ChangeMiddleName, new JsonHistoryValue
                {
                    Value = request.MiddleName,
                }, user.Id, currentUserId);
            }

            if (!string.IsNullOrWhiteSpace(request.Position) && user.Position != request.Position)
            {
                user.Position = request.Position;

                await AddHistory(db, EnumTypeHistory.ChangePosition, new JsonHistoryValue
                {
                    Value = request.Position,
                }, user.Id, currentUserId);
            }

            if (user.Salary != request.Salary)
            {
                user.Salary = request.Salary;

                await AddHistory(db, EnumTypeHistory.ChangeSalary, new JsonHistoryValue
                {
                    Value = Convert.ToString(request.Salary),
                }, user.Id, currentUserId);
            }

            if (request.WorkPlan != null && user.WorkPlan != request.WorkPlan)
            {
                user.WorkPlan = request.WorkPlan;

                await AddHistory(db, EnumTypeHistory.ChangeWorkPlan, new JsonHistoryValue
                {
                    WorkPlan = request.WorkPlan,
                }, user.Id, currentUserId);
            }
            //удаление старых документов и добавление новых

            var getDocuments = await db.Fotos.Where(c => c.TypeFoto == EnumTypeFoto.Document).ToListAsync();

            var files = new List<string>();

            if (request.Documents.Count > 0)
            {
                foreach (var docoment in getDocuments)
                {
                    await _fileService.Delete(new Client.Files.IService.Models.Request.DeleteFileVersionRequest
                    {
                        fileId = docoment.FotoId,
                        fileName = docoment.FotoUrl
                    }, docoment.Id);
                }

                foreach (var newDocument in request.Documents)
                {
                    var patch = await _fileService.Upload(new Client.Files.IService.Models.Request.UploadFileRequest
                    {
                        fileName = newDocument.Name,
                        base64 = newDocument.base64
                    }, request.UserId, EnumTypeFoto.Document, null);

                    files.Add(patch.GetUrl());
                }
            }

            var fotoUrl = await UpdateProfileFoto(db, request.UserId, currentUserId, request.UserFoto?.base64, request.UserFoto?.Name ?? "");

            await db.SaveChangesAsync();

            return new AdminUpdateResponse
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                MiddleName = user.MiddleName,
                Position = user.Position,
                Salary = user.Salary ?? 0,
                WorkPlan = user.WorkPlan,
                Documents = files,
                UserFotoUrl = fotoUrl.GetUrl(),
                AccessCanManage = user.AccessCanManage,
                Status = user.Status!.Value,
                TypeRole = user.TypeRole,
            };
        }

        private async Task<bool> AddHistory(DataContext db, EnumTypeHistory typeHistory, JsonHistoryValue historyValue, Guid userId, Guid createdUserId)
        {
            await db.Histories.AddAsync(new History
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                DateCreated = DateTime.UtcNow,
                CreatedUserId = createdUserId,
                Type = typeHistory,
                Value = JsonConvert.SerializeObject(historyValue)
            });

            return true;
        }

        /// <summary>
        /// Обновить данные владельца
        /// </summary>
        /// <returns></returns>
        public async Task<AdminUpdateOwnerResponse> UpdateOwner(AdminUpdateOwnerRequest request, Guid currentUserId)
        {
            await using var db = new DataContext(_options);

            var user = await db.Users
                .FirstOrDefaultAsync(c => c.Id == currentUserId && c.TypeRole == EnumUserRole.Owner);

            if (user == null)
                throw new Exception("Владелец не найден");

            user.FirstName = request.Name;

            //обновление фотографии владельца
            var fotoUrl = await UpdateProfileFoto(db, currentUserId, currentUserId, request.Foto?.base64, request.Foto?.Name ?? "");

            await db.SaveChangesAsync();

            return new AdminUpdateOwnerResponse
            {
                Foto = fotoUrl.GetUrl(),
                Name = user.FirstName,
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task<string?> UpdateProfileFoto(DataContext db, Guid userId, Guid createdUserId, string? foto, string name)
        {
            var getFoto = await db.Fotos.Where(c => c.UserId == userId && c.TypeFoto == EnumTypeFoto.Profile)
               .FirstOrDefaultAsync();

            if (string.IsNullOrWhiteSpace(foto) || string.IsNullOrWhiteSpace(name))
                return getFoto?.FotoUrl ?? "";

            if (getFoto == null && foto != null)
            {
                getFoto = new Foto();

                getFoto.FotoUrl = await _fileService.Upload(new Client.Files.IService.Models.Request.UploadFileRequest
                {
                    base64 = foto,
                    fileName = name
                }, userId, EnumTypeFoto.Profile, null);
            }
            else if (getFoto != null)
            {
                getFoto.FotoUrl = await _fileService.Upload(new Client.Files.IService.Models.Request.UploadFileRequest
                {
                    base64 = foto,
                    fileName = name
                }, userId, EnumTypeFoto.Profile, getFoto.Id);
            }
            else if (foto == null && getFoto != null)
            {
                await _fileService.Delete(new Client.Files.IService.Models.Request.DeleteFileVersionRequest
                {
                    fileId = getFoto.FotoId,
                    fileName = getFoto.FotoUrl
                }, getFoto.Id);
            }

            if (foto != null)
                await AddHistory(db, EnumTypeHistory.ChangeFotoProfile, new JsonHistoryValue
                {
                    Value = getFoto?.FotoUrl,
                }, userId, createdUserId);

            return getFoto == null ? null : getFoto.FotoUrl;
        }

        /// <summary>
        /// Переместить сотрудника в другую организацию
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Move(AdminMoveRequest request, Guid currentUserId)
        {
            await using var db = new DataContext(_options);

            request.OwnerId = request.OwnerId ?? currentUserId;

            // Проверка, что пользователь доступен текущему
            var getAccessUserIds = await GetAccessUserIds(db, new List<Guid> { currentUserId });
            getAccessUserIds.Add(currentUserId);

            var checkAccessUserId = getAccessUserIds.Contains(request.UserId);

            var checkAccessOwnerId = request.OwnerId.HasValue ? getAccessUserIds.Contains(request.UserId) : true;

            if (!checkAccessUserId || !checkAccessOwnerId)
                throw new Exception("Нет доступа к одному из пользователей пользователям");

            var checkOwnerRole = await db.Users.AnyAsync(c => c.Id == request.OwnerId && c.TypeRole != EnumUserRole.Employee);

            if (!checkOwnerRole)
                throw new Exception("Невозможно переместить пользователя к сотруднику.");

            if (!await CheckCorrectMoveUser(db,request.OwnerId.Value,request.UserId))
            {
                throw new Exception("Вы не можете переместить сотрудника в его подчиненого.");
            }

            var user = await db.Users
                .FirstOrDefaultAsync(c => c.Id == request.UserId);

            if (user == null)
                throw new Exception("Пользователь не найден");

            user.OwnerId = request.OwnerId;

            if (user.OwnerId != request.OwnerId)
                await AddHistory(db, EnumTypeHistory.ChangeDepartment, new JsonHistoryValue
                {
                    Value = request.OwnerId.Value.ToString(),
                }, user.Id, currentUserId);

            await db.SaveChangesAsync();

            return true;
        }

        private async Task<bool> CheckCorrectMoveUser(DataContext data,Guid userId,Guid ownerId)
        {
            var getUserIds = await data.Users.Where(c => c.OwnerId == ownerId)
                .Select(c => new
                {
                    c.Id,
                    c.OwnerId
                }).ToListAsync();

            bool response = true;

            foreach (var user in getUserIds)
            {
                if (user.Id == userId)
                    return false;

                response = await CheckCorrectMoveUser(data, userId, user.Id);
            }

            return response;
        }
    }
}
