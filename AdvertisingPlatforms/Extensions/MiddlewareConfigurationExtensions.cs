using AdvertisingPlatforms.Web.Filters;

namespace AdvertisingPlatforms.Web.Extensions
{
    public static class MiddlewareConfigurationExtensions
    {
        public static IApplicationBuilder UseApplicationMiddleware(this WebApplication app)
        {
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseCors("AllowAll");
            app.UseSwagger();
            app.UseSwaggerUI();
            app.MapControllers();

            return app;
        }
    }
}
