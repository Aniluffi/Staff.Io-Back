namespace Client.Files.IService.Models.Response
{
    public class DeleteFileResponse
    {
        public Error error { get; set; }
    }

    public class Error
    {
        public int code { get; set; }
        public string message { get; set; }
    }
}
