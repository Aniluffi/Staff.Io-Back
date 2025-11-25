namespace Client.Files.IService.Models.Request
{
    /// <summary>
    /// модель запроса для копирования файла
    /// </summary>
    public class CopyFileRequest
    {
        /// <summary>
        /// айди копируемого файла
        /// </summary>
        public string Id { get; set; }
    }
}
