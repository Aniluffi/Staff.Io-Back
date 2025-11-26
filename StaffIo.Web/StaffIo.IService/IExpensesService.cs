using StaffIo.IService.Models.ExpensesService.Response;

namespace StaffIo.IService
{
    public interface IExpensesService
    {
        /// <summary>
        /// Получение расходов на зарплаты сотрудников
        /// </summary>
        /// <returns></returns>
        Task<ExpensesGetResponse> Get(Guid currentUserId);
    }
}
