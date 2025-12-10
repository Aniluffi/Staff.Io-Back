using Http.Client.Common;
using Http.Client.IService;
using Http.Client.Models;
using System.Text;

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

        public async Task<BaseResponse<TResponse>> SendAsync<TResponse, TRequest>(
     string method,
     HttpMethod httpMethod,
     TRequest request)
        {
            try
            {
                // Создаём HttpRequest
                var httpRequest = new HttpRequestMessage(httpMethod, _basePatch + method);

                // Добавляем заголовок авторизации
                if (!string.IsNullOrWhiteSpace(_header.TypeAuth))
                    httpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(_header.TypeAuth, _header.AccessToken);
                else
                {
                    httpRequest.Headers.TryAddWithoutValidation("Authorization", _header.AccessToken);
                }

                var json = System.Text.Json.JsonSerializer.Serialize(request);
                httpRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");

                // Отправка запроса
                var httpResponse = await _httpClient.SendAsync(httpRequest);

                var response = new BaseResponse<TResponse>();

                // Читаем тело ответа
                var content = await httpResponse.Content.ReadAsStringAsync();

                if (httpResponse.IsSuccessStatusCode)
                {
                    response.Data = System.Text.Json.JsonSerializer.Deserialize<TResponse>(
                        string.IsNullOrWhiteSpace(content) ? "{}" : content
                    );
                }
                else
                {
                    response.ErrorMessage = $"HTTP {(int)httpResponse.StatusCode} ({httpResponse.ReasonPhrase}): {content}";
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
