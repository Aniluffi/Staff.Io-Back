namespace Client.Files.Options
{
    /// <summary>
    /// настройки для авторизации google
    /// </summary>
    public class GoogleAuthOptions
    {
        public string ClientEmail { get; set; }
        public string PrivateKey { get; set; }
        public string BasePatch { get; set; } = @"https://www.googleapis.com/drive/v3";
        public string BasePatchUpload { get; set; } = @"https://www.googleapis.com/upload/drive/v3";
        public string BaseFolder { get; set; }
    }
}
