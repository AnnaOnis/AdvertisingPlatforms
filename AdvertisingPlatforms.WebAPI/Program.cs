using AdvertisingPlatforms.Base.Constants;
using AdvertisingPlatforms.Web.Configurations;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

builder.AddLogging();

try
{
     Log.Information(LogMessages.APP_START);

    builder.Services.ConfigureApplicationServices(builder.Configuration);

    var app = builder.Build();

    app.ConfigureApplicationMiddleware();

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, LogMessages.APP_ERROR);
}
finally
{
    Log.Information(LogMessages.APP_STOP);
    await Log.CloseAndFlushAsync();
}
