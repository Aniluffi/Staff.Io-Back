using Client.Files.IService;
using Client.Files.Options;
using Client.Files.Service;
using Http.Client.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Files.Extensions
{
    public static class SturtupExstension
    {
        public static void AddFileService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient();

            services.Configure<BackBazeB2Options>(configuration.GetSection(nameof(BackBazeB2Options)));

            services.AddTransient<IBackblazeAuthService, BackblazeAuthService>();
            services.AddTransient<IFileB2Service, FileB2Service>();
        }
    }
}
