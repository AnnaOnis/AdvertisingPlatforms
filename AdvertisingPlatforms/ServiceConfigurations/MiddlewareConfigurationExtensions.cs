using AdvertisingPlatforms.Web.Filters;

namespace AdvertisingPlatforms.Web.ServiceConfigurations
{
    public static class MiddlewareConfigurationExtensions
    {
        public static IApplicationBuilder ConfigureApplicationMiddleware(this WebApplication app)
        {
            app.UseCors("AllowAll");

            ConfigureMiddleware(app);

            ConfigureSwagger(app);

            ConfigureControllers(app);

            return app;
        }

        private static void ConfigureMiddleware(IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionHandlingMiddleware>();
        }

        private static void ConfigureSwagger(IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        private static void ConfigureControllers(WebApplication app)
        {
            app.MapControllers();
        }
    }
}
