using Http.Client.Common;
using Http.Client.IService;
using Http.Client.Models;

namespace Http.Client.Service
{
    public class HttpClient : IHttpClient
    {
        private readonly System.Net.Http.HttpClient _httpClient;

        private readonly string _basePatch;

        private readonly Header _header;

        public HttpClient(string basePatch, Header header)
        {
            _httpClient = new System.Net.Http.HttpClient();
            _header = header;
            _basePatch = basePatch;
        }

        public async Task<BaseResponse<TResponse>> SendAsync<TResponse, TRequest>(string method, HttpMethod httpMethod, TRequest request, byte[]? file = null)
        {
            try
            {
                var httpRequest = new HttpRequestMessage(httpMethod, _basePatch + method);

                httpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(_header.TypeAuth, _header.AccessToken);

                var boundary = "my-boundary-" + Guid.NewGuid();
                var multipartContent = new MultipartContent("related", boundary);

                // Первая часть — JSON с метаданными
                var metadataContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(request));

                metadataContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                multipartContent.Add(metadataContent);

                if (file != null)
                {
                    // Вторая часть — байты файла
                    var fileContent = new ByteArrayContent(file);

                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(_header.MediaType);

                    multipartContent.Add(fileContent);

                    httpRequest.Content = multipartContent;
                }

                var requestResponse = await _httpClient.SendAsync(httpRequest);

                var response = new BaseResponse<TResponse>();

                if (requestResponse.IsSuccessStatusCode)
                {
                    var jsonResponse = await requestResponse.Content.ReadAsStringAsync();

                    var getResponse = System.Text.Json.JsonSerializer.Deserialize<TResponse>(string.IsNullOrWhiteSpace(jsonResponse) ? @"{}" : jsonResponse);

                    response.Data = getResponse;
                }
                else
                {
                    // Читаем тело ответа
                    var errorBody = await requestResponse.Content.ReadAsStringAsync();

                    // Формируем удобное сообщение
                    var message = $"HTTP {(int)requestResponse.StatusCode} ({requestResponse.ReasonPhrase}): {errorBody}";

                    response.ErrorMessage = message;
                }

                return response;
            }
            catch (Exception ex)
            {
                return new BaseResponse<TResponse>
                {
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}
