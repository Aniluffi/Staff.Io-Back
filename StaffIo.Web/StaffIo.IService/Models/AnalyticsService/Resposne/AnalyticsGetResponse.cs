namespace StaffIo.IService.Models.AnalyticsService.Resposne
{
    /// <summary>
    /// Ответ на получение аналитики
    /// </summary>
    public class AnalyticsGetResponse
    {
        /// <summary>
        /// Средняя зарплата сотрудников
        /// </summary>
        public decimal AverageSalary { get; set; }

        /// <summary>
        /// Коэффициент текучести кадров
        /// </summary>
        public double TurnoverRate { get; set; }

        /// <summary>
        /// Количество новых сотрудников
        /// </summary>
        public int NewEmployees { get; set; }

        /// <summary>
        /// Количество уволенных сотрудников
        /// </summary>
        public int FiredEmployees { get; set; }
    }
}
