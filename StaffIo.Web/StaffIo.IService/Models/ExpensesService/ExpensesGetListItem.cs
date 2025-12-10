namespace StaffIo.IService.Models.ExpensesService
{
    public class ExpensesGetListItem
    {
        /// <summary>
        /// Идентификатор сотрудника
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// ФИО сотрудника
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Зарплата сотрудника
        /// </summary>
        public decimal Salary { get; set; }

        /// <summary>
        /// Дата выплаты
        /// </summary>
        public DateTime DatePay { get; set; }

        public string? FotoUrl { get; set; }
    }
}
