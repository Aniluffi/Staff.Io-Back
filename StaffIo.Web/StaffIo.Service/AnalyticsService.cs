using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StaffIo.Data;
using StaffIo.Data.Enums;
using StaffIo.Data.JsonModels;
using StaffIo.IService;
using StaffIo.IService.Models.AnalyticsService.Request;
using StaffIo.IService.Models.AnalyticsService.Resposne;
using StaffIo.IService.Models.ExpensesService;
using StaffIo.Service.Models;
using System.Runtime.ConstrainedExecution;

namespace StaffIo.Service
{
    public class AnalyticsService : IAnalyticsService
    {
        public DbContextOptions<DataContext> _options { get; set; }

        public AnalyticsService(DbContextOptions<DataContext> options)
        {
            _options = options;
        }

        /// <summary>
        /// Получение аналитики
        /// </summary>
        /// <returns></returns>S
        public async Task<AnalyticsGetResponse> Get(AnalyticsGetRequest request, Guid currentUserId)
        {
            await using var db = new DataContext(_options);

            var employees = await db.Users
                .Where(e => e.OwnerId == currentUserId)
                .Select(c => new AnalyticsUserShortListItem
                {
                    UserId = c.Id,
                    Status = c.Status!.Value,
                    DateCreated = c.DateCreatrd,
                    DateDeleted = c.DateDeleted
                })
                .ToListAsync();

            var userIds = employees.Select(e => e.UserId).ToList();

            var subEmployees = await GetAllUsers(db, userIds);

            employees.AddRange(subEmployees);

            userIds = employees.Select(e => e.UserId).ToList();

            var getSalarys = await db.Histories
                .Where(c => userIds.Contains(c.UserId)
                    && c.Type == EnumTypeHistory.ChangeSalary)
                .GroupBy(c => c.UserId)
                .Select(c => c.OrderByDescending(s => s.DateCreated)
                    .Where(s => s.DateCreated <= request.DateEnd && s.Type == EnumTypeHistory.ChangeSalary)
                    .Select(s => new AnalyticsSalaryListItem
                    {
                        Value = s.Value,
                        Salary = (decimal)0
                    })
                    .FirstOrDefault() ?? new AnalyticsSalaryListItem
                    {
                        Value = "",
                        Salary = (decimal)0
                    })
                .ToListAsync();

            foreach (var salary in getSalarys)
            {
                if (salary != null)
                {
                    var getSalary = string.IsNullOrWhiteSpace(salary.Value) ? "0" : JsonConvert.DeserializeObject<JsonHistoryValue>(salary.Value)!.Value;

                    salary.Salary = Convert.ToDecimal(getSalary);
                }
            }

            var averageSalary = getSalarys.Count > 0 ? getSalarys.Select(c => c.Salary).Average() : 0;

            var newEmployees = employees
                .Where(e => e.DateCreated >= request.DateStart && e.DateCreated <= request.DateEnd && (e.DateDeleted.HasValue ? e.DateDeleted > request.DateEnd : true))
                .Count();

            var firedEmployees = employees.Where(e => e.DateDeleted.HasValue && e.DateDeleted >= request.DateStart && e.DateDeleted <= request.DateEnd)
                .Count();

            var turnoverRate = (double)0;

            int getDaysDifference = (int)(request.DateEnd - request.DateStart).TotalDays;

            var startCount = employees.Count > 0 ? 
                employees.Where(c => c.DateCreated < request.DateStart && (c.DateDeleted.HasValue ? c.DateDeleted > request.DateStart : true)).Count() : 0;

            if (getDaysDifference != 0)
            {
                if (getDaysDifference >= 365)
                {
                    var dateStart = request.DateStart;

                    var numberPeoplePerMonth = new List<int>();

                    while (dateStart <= request.DateEnd)
                    {
                        var dateEnd = dateStart.AddMonths(1);

                        var countAddEmployees = employees
                            .Where(e => e.DateCreated >= dateStart && e.DateCreated <= dateEnd && e.Status == EnumUserStatus.Active)
                            .Count();

                        var countFiredEmployees = employees
                            .Where(e => e.DateDeleted.HasValue && e.DateDeleted >= dateStart && e.DateDeleted <= dateEnd && e.Status == EnumUserStatus.Deactivated)
                            .Count();

                        var countPeople = startCount + countAddEmployees - countFiredEmployees;

                        startCount = countPeople;

                        numberPeoplePerMonth.Add(countPeople);

                        dateStart = dateStart.AddMonths(1);
                    }

                    var averagePeopleCount = numberPeoplePerMonth.Average();

                    turnoverRate = (firedEmployees / averagePeopleCount) * 100;
                } else if(getDaysDifference >= 2) 
                {
                    // Численность на конец периода (Срез на request.DateEnd)
                    var endCount = employees
                        .Count(e => e.DateCreated <= request.DateEnd &&
                                    (!e.DateDeleted.HasValue || e.DateDeleted > request.DateEnd));

                    // Среднее арифметическое: (Начало + Конец) / 2
                    var averagePeopleCount = (startCount + endCount) / 2.0;

                    turnoverRate = averagePeopleCount == 0.0
                        ? 0.0
                        : ((double)firedEmployees / averagePeopleCount) * 100;
                }
                else
                {
                    turnoverRate = 0;
                }
            }

            var response = new AnalyticsGetResponse
            {
                AverageSalary = averageSalary,
                NewEmployees = newEmployees,
                FiredEmployees = firedEmployees,
                TurnoverRate = turnoverRate
            };

            return response;
        }

        private async Task<List<AnalyticsUserShortListItem>> GetAllUsers(DataContext data, List<Guid> userIds)
        {
            var getUsers = await data.Users
                .Where(u => u.OwnerId.HasValue && u.Status == EnumUserStatus.Active && userIds.Contains(u.OwnerId.Value))
                .Select(u => new AnalyticsUserShortListItem
                {
                    UserId = u.Id,
                    Status = u.Status!.Value,
                    DateCreated = u.DateCreatrd,
                    DateDeleted = u.DateDeleted
                })
                .ToListAsync();

            var getUsersIds = getUsers.Select(u => u.UserId).ToList();

            if (getUsersIds.Count == 0)
                return getUsers;

            var getSubUsers = await GetAllUsers(data, getUsersIds);

            getUsers.AddRange(getSubUsers);

            return getUsers;
        }
    }
}
