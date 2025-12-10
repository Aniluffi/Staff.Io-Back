using Microsoft.EntityFrameworkCore;
using StaffIo.Data;
using StaffIo.Data.Enums;
using StaffIo.IService;
using StaffIo.IService.Models.EmployeesServices;
using StaffIo.IService.Models.EmployeesServices.Request;
using StaffIo.IService.Models.EmployeesServices.Response;
using StaffIo.Service.Extensions;

namespace StaffIo.Service
{
    public class EmployeesService : IEmployeesService
    {
        public DbContextOptions<DataContext> _options;

        public EmployeesService(DbContextOptions<DataContext> options)
        {
            _options = options;
        }

        /// <summary>
        /// Получить список сотрудников
        /// </summary>
        /// <returns></returns>
        public async Task<EmployeesGetListResponse> GetList(EmployeesGetListRequest request, Guid currentUserId)
        {
            await using var db = new DataContext(_options);

            var getEmployeeList = new List<EmployeeListItem>();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                getEmployeeList = await GetFilteredTree(db, currentUserId, request.Status, request.Search);
            }
            else
            {
                var employesQueue = db.Users.Where(c => c.OwnerId == currentUserId).AsQueryable();

                if (request.Status.HasValue)
                {
                    employesQueue = employesQueue.Where(c => c.Status == request.Status.Value);
                }

                getEmployeeList = await employesQueue.Select(c => new EmployeeListItem
                {
                    UserId = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    OwnerId = c.OwnerId,
                    MiddleName = c.MiddleName,
                    Status = c.Status!.Value,
                    Salary = c.Salary ?? 0,
                    IsAdmin = c.TypeRole == Data.Enums.EnumUserRole.Admin,
                    AccessCanManage = c.TypeRole == EnumUserRole.Owner ? true : c.AccessCanManage
                }).ToListAsync();

                foreach (var employee in getEmployeeList)
                {
                    employee.Items = await GetTree(db, employee.UserId, request.Status);
                }
            }

            return new EmployeesGetListResponse
            {
                Items = getEmployeeList
            };
        }

        private async Task<List<EmployeeListItem>> GetTree(DataContext db, Guid? ownerId, EnumUserStatus? status)
        {
            var getUsers = await db.Users
                .Where(u => ownerId == u.OwnerId.Value && (status.HasValue ? u.Status == status.Value : true))
                .Select(c => new EmployeeListItem
                {
                    UserId = c.Id,
                    OwnerId = c.OwnerId,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    MiddleName = c.MiddleName,
                    Status = c.Status!.Value,
                    Salary = c.Salary ?? 0,
                    IsAdmin = c.TypeRole == Data.Enums.EnumUserRole.Admin,
                    AccessCanManage = c.TypeRole == EnumUserRole.Owner ? true : c.AccessCanManage,
                    Items = new List<EmployeeListItem>()
                })
                .ToListAsync();

            foreach (var user in getUsers)
            {
                user.Items = await GetTree(db, user.UserId, status);
            }

            return getUsers;
        }

        private async Task<List<EmployeeListItem>> GetFilteredTree(
    DataContext db,
    Guid? ownerId,
    EnumUserStatus? status,
    string? searchTerm)
        {
            // === 1. ЗАГРУЗКА: Получаем всех пользователей (включая корневых) одним запросом. ===
            var allUsersQuery = db.Users.Where(c => c.OwnerId.HasValue).AsNoTracking();

            // 💡 ИСПРАВЛЕНИЕ: Удален некорректный фильтр .Where(c => c.OwnerId.HasValue)
            if (status.HasValue)
            {
                allUsersQuery = allUsersQuery.Where(u => u.Status == status.Value);
            }

            var allItems = await allUsersQuery
                .Select(c => new EmployeeListItem
                {
                    UserId = c.Id,
                    OwnerId = c.OwnerId,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    MiddleName = c.MiddleName,
                    Status = c.Status!.Value,
                    Salary = c.Salary ?? 0,
                    IsAdmin = c.TypeRole == Data.Enums.EnumUserRole.Admin,
                    AccessCanManage = c.TypeRole == EnumUserRole.Owner ? true : c.AccessCanManage,
                    Items = new List<EmployeeListItem>()
                })
                .ToListAsync();

            if (!allItems.Any())
                return new List<EmployeeListItem>();

            // Словарь для быстрого доступа ко всем узлам
            var usersDict = allItems.ToDictionary(u => u.UserId);
            var nodesToInclude = new HashSet<Guid>();

            // === 2. ПОИСК "СНИЗУ ВВЕРХ": Собираем ТОЛЬКО пути к совпадениям ===

            // 2а. Находим совпадения и собираем их предков
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var normalizedSearchTerm = searchTerm.ToLower();

                var matchingUsers = allItems.Where(u =>
                    u.FirstName.ToLower().Contains(normalizedSearchTerm) ||
                    (u.LastName != null && u.LastName.ToLower().Contains(normalizedSearchTerm)) ||
                    (u.MiddleName != null && u.MiddleName.ToLower().Contains(normalizedSearchTerm))
                ).ToList();

                // Поднимаемся вверх от найденных узлов, чтобы добавить всех предков (ПУТЬ)
                foreach (var user in matchingUsers)
                {
                    var current = user;
                    while (current != null)
                    {
                        if (!nodesToInclude.Add(current.UserId)) break;

                        if (current.OwnerId.HasValue && usersDict.TryGetValue(current.OwnerId.Value, out var owner))
                        {
                            current = owner;
                        }
                        else
                        {
                            current = null;
                        }
                    }
                }
            }
            else
            {
                // Если поиска нет, включаем все узлы (возвращаем полное дерево)
                nodesToInclude = allItems.Select(u => u.UserId).ToHashSet();
            }

            // ❌ УДАЛЕН ШАГ 2б (AddDescendants): Больше не включаем потомков.

            // === 3. ПОСТРОЕНИЕ ДЕРЕВА ===

            // Фильтруем список, оставляя только узлы, составляющие пути к найденным элементам
            var filteredList = allItems.Where(u => nodesToInclude.Contains(u.UserId)).ToList();

            // Словарь отфильтрованных узлов для быстрой привязки
            var filteredDict = filteredList.ToDictionary(u => u.UserId);
            var rootNodes = new List<EmployeeListItem>();

            // Очищаем списки Items для чистого построения
            filteredList.ForEach(u => u.Items = new List<EmployeeListItem>());

            foreach (var user in filteredList)
            {
                // Пытаемся найти родителя в отфильтрованном списке
                if (user.OwnerId.HasValue && filteredDict.TryGetValue(user.OwnerId.Value, out var parent))
                {
                    // Если родитель найден (т.е., он является частью пути), добавляем текущий узел как его потомок.
                    parent.Items.Add(user);
                }
                // Если родителя нет в отфильтрованном списке, или его OwnerId совпадает с корневым фильтром,
                // считаем узел корневым для возвращаемого дерева.
                else if (user.OwnerId == ownerId)
                {
                    rootNodes.Add(user);
                }
            }

            // Возвращаем список корневых узлов
            return rootNodes;
        }

        /// <summary>
        /// Получить детальный профиль пользователя
        /// </summary>
        /// <returns></returns>
        public async Task<EmployeesGetDetailResponse> GetDetail(EmployeesGetDetailRequest request, Guid currentUserId)
        {
            await using var db = new DataContext(_options);

            var getAccessUserIds = await GetAccessUserIds(db, new List<Guid> { currentUserId });

            var checkAccess = getAccessUserIds.Contains(request.UserId);

            if (!checkAccess)
                throw new Exception("Нет доступа к пользователю");

            var user = await db.Users
                .Where(u => u.Id == request.UserId)
                .Select(u => new EmployeesGetDetailResponse
                {
                    UserId = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    MiddleName = u.MiddleName,
                    Status = u.Status!.Value,
                    Salary = u.Salary ?? 0,
                    TypeRole = u.TypeRole,
                    Position = u.Position,
                    AccessCanManage = u.AccessCanManage,
                    WorkPlan = u.WorkPlan,
                    Documents = new List<string>(),
                    UserFotoUrl = string.Empty,
                }).FirstOrDefaultAsync();

            if (user == null)
                throw new Exception("Пользователь не найден");

            var getFotoUrls = await db.Fotos
                .Where(uf => uf.UserId == request.UserId)
                .Select(uf => new
                {
                    FotoUrl = uf.FotoUrl.GetUrl(),
                    uf.TypeFoto
                })
                .ToListAsync();

            user.UserFotoUrl = getFotoUrls.Where(c => c.TypeFoto == EnumTypeFoto.Profile).Select(c => c.FotoUrl).FirstOrDefault();

            user.Documents = getFotoUrls
                .Where(c => c.TypeFoto == EnumTypeFoto.Document)
                .Select(c => c.FotoUrl)
                .ToList();

            return user;
        }

        private async Task<List<Guid>> GetAccessUserIds(DataContext data, List<Guid> userIds)
        {
            var getUserIds = await data.Users
                .Where(u => userIds.Contains(u.OwnerId.Value))
                .Select(u => u.Id)
                .ToListAsync();

            if (getUserIds.Count == 0)
                return getUserIds;

            var getSubUsers = await GetAccessUserIds(data, getUserIds);

            getUserIds.AddRange(getSubUsers);

            return getUserIds;
        }

        /// <summary>
        /// Получить профиль текущего пользователя (кроме админа)
        /// </summary>
        /// <returns></returns>
        public async Task<EmployeesGetCurrentProfileResponse> GetCurrentProfile(Guid currentUserId)
        {
            await using var db = new DataContext(_options);

            var user = await db.Users
                .Where(u => u.Id == currentUserId && u.OwnerId.HasValue)
                .Select(u => new EmployeesGetCurrentProfileResponse
                {
                    UserId = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    MiddleName = u.MiddleName,
                    Status = u.Status!.Value,
                    Salary = u.Salary ?? 0,
                    TypeRole = u.TypeRole,
                    Position = u.Position,
                    AccessCanManage = u.AccessCanManage,
                    WorkPlan = u.WorkPlan,
                    Documents = new List<string>(),
                    UserFotoUrl = string.Empty,
                }).FirstOrDefaultAsync();

            if (user == null)
                throw new Exception("Пользователь не найден");

            var getFotoUrls = await db.Fotos
                .Where(uf => uf.UserId == currentUserId)
                .Select(uf => new
                {
                    FotoUrl = uf.FotoUrl.GetUrl(),
                    uf.TypeFoto
                })
                .ToListAsync();

            user.UserFotoUrl = getFotoUrls.Where(c => c.TypeFoto == EnumTypeFoto.Profile).Select(c => c.FotoUrl).FirstOrDefault();

            user.Documents = getFotoUrls
                .Where(c => c.TypeFoto == EnumTypeFoto.Document)
                .Select(c => c.FotoUrl)
                .ToList();

            return user;
        }
    }
}
