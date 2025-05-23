using AdvertisingPlatforms.DAL;
using AdvertisingPlatforms.Domain.Abstractions;
using AdvertisingPlatforms.DAL.Entities;
using AdvertisingPlatforms.Domain.Validators;
using AdvertisingPlatforms.Domain.Parser;
using AdvertisingPlatforms.Domain.Services;
using AdvertisingPlatforms.DAL.Abstractions;

namespace AdvertisingPlatforms.Web.ServiceConfigurations
{
    public static class ServiceConfigurationExtensions
    {
        public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services)
        {
            AddInfrastructure(services);
            AddApplicationComponents(services);

            return services;
        }

        private static void AddInfrastructure(IServiceCollection services)
        {
            AddLogging(services);
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        private static void AddApplicationComponents(IServiceCollection services)
        {
            AddDomainServices(services);
            AddRepositories(services);
            AddDataStorage(services);
        }

        private static void AddLogging(IServiceCollection services)
        {
            services.AddLogging(logging =>
            {
                logging.AddConsole();
                logging.AddDebug();
            });
        }

        private static void AddDomainServices(IServiceCollection services)
        {       
            services.AddScoped<IAdvertisingPlatformService, AdvertisingPlatformService>();
            services.AddSingleton<IAdvertisingPlatformParser, PlatformsFileParser>();
            services.AddScoped<IValidator<Location>, LocationValidator>()
                .AddScoped<IValidator<AdvertisingPlatform>, AdvertisingPlatformValidator>(); 
        }

        private static void AddRepositories(IServiceCollection services)
        {
            services.AddScoped<IAdvertisingPlatformRepository, AdvertisingPlatformRepositoryInMemory>();
        }

        private static void AddDataStorage(IServiceCollection services)
        {
            services.AddSingleton<InMemoryAdvertisingPlatformStorage>();
        }
    }
}
