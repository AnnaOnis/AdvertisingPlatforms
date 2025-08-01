using AdvertisingPlatforms.Web.Logging;
using Serilog;

namespace AdvertisingPlatforms.Web.Configurations
{
    public static class LoggerConfiguration
    {
        public static void AddLogging(this WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog((context, config) =>
            {
                config.ReadFrom.Configuration(context.Configuration);
            });
            builder.Services.Configure<LoggingSettings>(builder.Configuration.GetSection("LogOptions"));

            Log.Logger = new Serilog.LoggerConfiguration()
               .WriteTo.Console()
               .CreateBootstrapLogger();
        }
    }
}
