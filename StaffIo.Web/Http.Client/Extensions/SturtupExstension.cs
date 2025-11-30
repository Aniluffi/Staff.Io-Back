using Http.Client.IService;
using Microsoft.Extensions.DependencyInjection;

namespace Http.Client.Extensions
{
    public static class SturtupExstension
    {
        public static void AddHttpClient(this IServiceCollection serviceCollection)
        {
            //serviceCollection.AddTransient<IHttpClient, Http.Client.Service.HttpClient>();
        }
    }
}
