using Http.Client.Common;
using Http.Client.Models;

namespace Http.Client.IService
{
    /// <summary>
    /// интерфейс для запросов на Api
    /// </summary>
    public interface IHttpClient
    {
        Task<BaseResponse<TResponse>> SendAsync<TResponse, TRequest>(string method, HttpMethod httpMethod, TRequest request);
    }
}
