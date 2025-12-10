namespace Client.Files.Options
{
    /// <summary>
    /// настройки для авторизации google
    /// </summary>
    public class BackBazeB2Options
    {
        public string ApplicationKeyId { get; set; } = "5dda02ade645";
        public string ApplicationKey { get; set; } = "0037a95de2a05a9a0f0d93e18ccb50b291362dbe37";
        public string AuthPatch { get; set; } = @"https://api.backblazeb2.com/b2api/v4/b2_authorize_account";
        public string BasketId { get; set; } = "75fdcd1a4022faad9ea60415";
    }
}
