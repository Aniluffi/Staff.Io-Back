namespace StaffIo.IService.Models.ExpensesService.Response
{
    public class ExpensesGetResponse
    {
        /// <summary>
        /// Список расходов
        /// </summary>
        public List<ExpensesGetListItem> Items { get; set; }

        /// <summary>
        /// Сумма расходов
        /// </summary>
        public decimal ExpensesSum { get; set; }
    }
}
