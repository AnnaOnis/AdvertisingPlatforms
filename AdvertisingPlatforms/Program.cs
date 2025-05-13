using AdvertisingPlatforms.Web.ServiceConfigurations;

namespace AdvertisingPlatforms
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.ConfigureApplicationServices();

            var app = builder.Build();

            app.ConfigureApplicationMiddleware();

            app.Run();
        }
    }
}
