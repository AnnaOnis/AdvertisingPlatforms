using AdvertisingPlatforms.DAL;
using AdvertisingPlatforms.Domain.Entities;
using AdvertisingPlatforms.Domain.Interfaces;
using AdvertisingPlatforms.Domain.Repositories;
using AdvertisingPlatforms.Domain.Validators;
using AdvertisingPlatforms.Parser;
using AdvertisingPlatforms.Services;

namespace AdvertisingPlatforms.Web.Extensions
{
    public static class ServiceConfigurationExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddLogging(logging =>
            {
                logging.AddConsole();
                logging.AddDebug();
            });

            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddScoped<IAdvertisingPlatformService, AdvertisingPlatformService>();
            services.AddSingleton<IAdvertisingPlatformParser, PlatformsFileParser>();
            services.AddScoped<IValidator<Location>, LocationValidator>();
            services.AddScoped<IValidator<AdvertisingPlatform>, AdvertisingPlatformValidator>();
            services.AddScoped<IAdvertisingPlatformRepository, AdvertisingPlatformRepositoryInMemory>();
            services.AddSingleton<InMemoryAdvertisingPlatformStorage>();

            return services;
        }
    }
}
