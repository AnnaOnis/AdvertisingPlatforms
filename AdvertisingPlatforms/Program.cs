using AdvertisingPlatforms.Base.Constants;
using AdvertisingPlatforms.Web.Configurations;
using Serilog;

namespace AdvertisingPlatforms
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.AddLogging();

            try
            {
                Log.Information(LogMessages.APP_START);

                builder.Services.ConfigureApplicationServices(builder.Configuration);

                var app = builder.Build();

                app.ConfigureApplicationMiddleware();

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, LogMessages.APP_ERROR);
            }
            finally
            {
                Log.Information(LogMessages.APP_STOP);
                Log.CloseAndFlush();
            }
        }
    }
}
