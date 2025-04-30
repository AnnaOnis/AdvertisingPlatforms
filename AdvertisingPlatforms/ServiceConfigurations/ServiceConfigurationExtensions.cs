using AdvertisingPlatforms.DAL;
using AdvertisingPlatforms.Domain.Abstractions;
using AdvertisingPlatforms.Domain.Entities;
using AdvertisingPlatforms.Domain.Interfaces;
using AdvertisingPlatforms.Domain.Validators;
using AdvertisingPlatforms.Parser;
using AdvertisingPlatforms.Services;

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
            AddValidators(services);
            AddRepositories(services);
            AddDataStorage(services);
            AddParsers(services);
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
        }
        private static void AddParsers(IServiceCollection services)
        {
            services.AddSingleton<IAdvertisingPlatformParser, PlatformsFileParser>();
        }

        private static void AddValidators(IServiceCollection services)
        {
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
