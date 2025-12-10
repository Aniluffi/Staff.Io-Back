using System.Text.Json.Serialization;

namespace Client.Files.IService.Models.Response
{
    public class BackblazeAuthState
    {
        public string accountId { get; set; } = default!;

        public ApiInfo apiInfo { get; set; } = default!;

        public long? applicationKeyExpirationTimestamp { get; set; }

        public string authorizationToken { get; set; } = default!;

        public DateTime expiresAtUtc { get; set; }
    }

    public class ApiInfo
    {
        public StorageApi storageApi { get; set; } = default!;
    }

    public class StorageApi
    {
        [JsonPropertyName("absoluteMinimumPartSize")]
        public long AbsoluteMinimumPartSize { get; set; }

        public Allowed allowed { get; set; } = default!;

        public string apiUrl { get; set; } = default!;

        public string downloadUrl { get; set; } = default!;

        public long recommendedPartSize { get; set; }

        public string s3ApiUrl { get; set; } = default!;
    }

    public class Allowed
    {
        public object? buckets { get; set; }

        public List<string> capabilities { get; set; } = new();

        public object? namePrefix { get; set; }
    }
}
