using Client.Files.IService;
using Microsoft.EntityFrameworkCore;
using StaffIo.Data;
using StaffIo.IService;
using StaffIo.IService.Models.HistoryService;
using StaffIo.IService.Models.HistoryService.Request;
using StaffIo.IService.Models.HistoryService.Response;
using StaffIo.Service.Extensions;

namespace StaffIo.Service
{
    public class HistoryService : IHistoryService
    {
        private DbContextOptions<DataContext> _options;
        private IFileB2Service _fileService;

        public HistoryService(DbContextOptions<DataContext> options, IFileB2Service fileService)
        {
            _options = options;
            _fileService = fileService;
        }

        /// <summary>
        /// получение истории для пользователя
        /// </summary>
        /// <returns></returns>
        public async Task<HistoryGetResponse> Get(HistoryGetRequest request, Guid currentUserId)
        {
            await using var db = new DataContext(_options);

            // Проверка, что пользователь доступен текущему
            var getAccessUserIds = await GetAccessUserIds(db, new List<Guid> { currentUserId });

            getAccessUserIds.Add(currentUserId);

            var checkAccess = getAccessUserIds.Contains(request.UserId);

            if (!checkAccess)
                throw new Exception("Нет доступа к истории данного пользователя");

            var getHistories = await db.Histories
                .Where(r => r.UserId == request.UserId)
                .Select(c => new 
                {
                    Type = c.Type,
                    Value = c.Value,
                    c.CreatedUserId,
                    c.DateCreated
                }).ToListAsync();

            var userIds = getHistories.Select(c => c.CreatedUserId).Distinct().ToList();

            var getUsers = await db.Users.Where(c => userIds.Contains(c.Id))
                .Select(c => new
                {
                    c.Id,
                    FullName = c.FirstName + " " + (c.MiddleName ?? "") + " " + (c.LastName ?? "")
                }).ToListAsync();

            var getFotos = await db.Fotos.Where(c => c.TypeFoto == Data.Enums.EnumTypeFoto.Profile && userIds.Contains(c.UserId))
                .Select(c => new
                {
                    c.UserId,
                    c.FotoUrl,
                }).ToListAsync();

            var response = new List<HistoryGetListItem>();

            foreach (var history in getHistories)
            {
                var foto = getFotos.Where(c => c.UserId == history.CreatedUserId).Select(c => c.FotoUrl).FirstOrDefault();

                var fullName = getUsers.Where(c => c.Id == history.CreatedUserId).Select(c => c.FullName).FirstOrDefault();

                response.Add(new HistoryGetListItem
                {
                    Type = history.Type,
                    Value = history.Value,
                    FullNameUserCreated = fullName ?? "&&&&&&&&&",
                    FotoUrlUserCreated = foto.GetUrl(),
                    DateCreated = history.DateCreated
                });
            }

            return new HistoryGetResponse
            {
                Items = response.OrderByDescending(c => c.DateCreated).ToList(),
            };
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
    }
}
