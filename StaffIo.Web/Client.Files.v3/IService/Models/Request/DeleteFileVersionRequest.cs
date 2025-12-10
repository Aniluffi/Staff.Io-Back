namespace Client.Files.IService.Models.Request
{
    public class DeleteFileVersionRequest
    {
        /// <summary>
        /// номер файла
        /// </summary>
        public string fileId { get; set; }

        /// <summary>
        /// название файла
        /// </summary>
        public string fileName { get; set; }
    }
}
