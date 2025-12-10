namespace Client.Files.IService.Models.Response
{
    public class UploadFileResponse
    {
        public string fileId { get; set; } = default!;
        public string fileName { get; set; } = default!;
        public string bucketId { get; set; } = default!;
    }
}
