using AdvertisingPlatforms.Web.Middlewares;

namespace AdvertisingPlatforms.Web.Configurations
{
    public static class MiddlewareConfiguration
    {
        public static void ConfigureApplicationMiddleware(this WebApplication app)
        {
            app.UseCors("AllowAll");

            ConfigureMiddleware(app);

            ConfigureSwagger(app);

            ConfigureControllers(app);

        }

        private static void ConfigureMiddleware(IApplicationBuilder app)
        {    
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseMiddleware<CustomHttpLogingMiddleware>();
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
