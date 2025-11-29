namespace StaffIo.Data.JsonModels
{
    /// <summary>
    /// Значение истории в формате JSON
    /// </summary>
    public class JsonHistoryValue
    {
        /// <summary>
        /// Значение
        /// </summary>
        public string? Value { get; set; }

        /// <summary>
        /// список значений
        /// </summary>
        public List<string>? Values { get; set; }
    }
}
