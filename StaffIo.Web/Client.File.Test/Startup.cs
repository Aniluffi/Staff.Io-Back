using Client.Files.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Client.File.Test
{
    public class Startup
    {
        IConfiguration configuration = new ConfigurationBuilder()
              .SetBasePath(Environment.CurrentDirectory) // путь к проекту
              .AddJsonFile("appsettings.json", true,true)
              .Build();


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddFileService(configuration);
        }
    }
}
