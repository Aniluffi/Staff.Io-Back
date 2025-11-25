namespace Client.Files.IService.Models.Request
{
    public class UpdateFileRequest
    {
        /// <summary>
        /// айдт файла на google disck
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// новый файл
        /// </summary>
        public byte[] File { get; set; }

        public string mimeType { get; set; }
    }
}
