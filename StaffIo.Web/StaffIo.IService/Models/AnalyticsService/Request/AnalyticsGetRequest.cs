namespace StaffIo.IService.Models.AnalyticsService.Request
{
    /// <summary>
    /// Запрос на получение аналитики
    /// </summary>
    public class AnalyticsGetRequest
    {
        /// <summary>
        /// Дата начала
        /// </summary>
        public DateTime DateStart { get; set; }

        /// <summary>
        /// Дата окончания
        /// </summary>
        public DateTime DateEnd { get; set; }
    }
}
