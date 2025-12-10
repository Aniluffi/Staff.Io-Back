using Microsoft.EntityFrameworkCore;
using StaffIo.Data;
using StaffIo.Data.Enums;
using StaffIo.IService;
using StaffIo.IService.Models.ExpensesService;
using StaffIo.IService.Models.ExpensesService.Response;
using StaffIo.Service.Extensions;

namespace StaffIo.Service
{
    public class ExpensesService : IExpensesService
    {
        private DbContextOptions<DataContext> _options;

        public ExpensesService(DbContextOptions<DataContext> options)
        {
            _options = options;
        }

        /// <summary>
        /// Получение расходов на зарплаты сотрудников
        /// </summary>
        /// <returns></returns>
        public async Task<ExpensesGetResponse> Get(Guid currentUserId)
        {
            await using var context = new DataContext(_options);

            var getUsers = await context.Users
                .Where(u => u.Status == EnumUserStatus.Active && u.OwnerId == currentUserId)
                .Select(u => new ExpensesGetListItem
                {
                    UserId = u.Id,
                    FullName = $"{u.FirstName} {u.MiddleName} {u.LastName}",
                    Salary = u.Salary.HasValue ? u.Salary.Value : 0,
                    DatePay = DateTime.UtcNow.Date.AddMonths(1),
                    FotoUrl = u.Fotos.Where(c => c.TypeFoto == EnumTypeFoto.Profile).Select(c => c.FotoUrl.GetUrl()).FirstOrDefault()
                })
                .ToListAsync();

            var getUsersIds = getUsers.Select(u => u.UserId).ToList();

            var getSubUsers = await GetAllUsers(context, getUsersIds);

            getUsers.AddRange(getSubUsers);

            var response = new ExpensesGetResponse
            {
                Items = getUsers,
                ExpensesSum = getUsers.Sum(u => u.Salary)
            };

            return response;
        }

        private async Task<List<ExpensesGetListItem>> GetAllUsers(DataContext data,List<Guid> userIds)
        {
            var getUsers = await data.Users
                .Where(u => u.OwnerId.HasValue && u.Status == EnumUserStatus.Active && userIds.Contains(u.OwnerId.Value))
                .Select(u => new ExpensesGetListItem
                {
                    UserId = u.Id,
                    FullName = $"{u.FirstName} {u.MiddleName} {u.LastName}",
                    Salary = u.Salary.HasValue ? u.Salary.Value : 0,
                    DatePay = DateTime.UtcNow.Date.AddMonths(1),
                    FotoUrl = u.Fotos.Where(c => c.TypeFoto == EnumTypeFoto.Profile).Select(c => c.FotoUrl.GetUrl()).FirstOrDefault()
                })
                .ToListAsync();

            var getUsersIds = getUsers.Select(u => u.UserId).ToList();

            if(getUsersIds.Count == 0)
                return getUsers;

            var getSubUsers = await GetAllUsers(data, getUsersIds);

            getUsers.AddRange(getSubUsers);

            return getUsers;
        }
    }
}
