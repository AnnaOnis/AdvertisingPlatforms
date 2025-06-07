using AdvertisingPlatforms.Domain.Abstractions;
using AdvertisingPlatforms.DAL.Entities;
using AdvertisingPlatforms.Domain.Validators;
using AdvertisingPlatforms.Domain.Parser;
using AdvertisingPlatforms.Domain.Services;
using AdvertisingPlatforms.DAL.Abstractions;
using Microsoft.AspNetCore.HttpLogging;
using AdvertisingPlatforms.DAL.Repositories.InMemory;

namespace AdvertisingPlatforms.Web.Configurations
{
    public static class ServiceConfiguration
    {
        public static void ConfigureApplicationServices(this IServiceCollection services)
        {
            AddInfrastructure(services);
            AddApplicationComponents(services);
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

            services.AddHttpLogging
                (logging => { 
                    logging.LoggingFields = HttpLoggingFields.All;
                    logging.RequestBodyLogLimit = 4096;
                    logging.ResponseBodyLogLimit = 4096;
                    logging.CombineLogs = true;
                });
        }

        private static void AddDomainServices(IServiceCollection services)
        {       
            services.AddScoped<IAdvertisingPlatformService, AdvertisingPlatformService>();
            services.AddSingleton<IAdvertisingPlatformParser, PlatformsFileParser>();
            services.AddScoped<IValidator<Location>, LocationValidator>()
                .AddScoped<IValidator<AdvertisingPlatform>, AdvertisingPlatformValidator>()
                .AddScoped<IFileDataValidator<IFileData>, FileDataValidator>(); 
        }

        private static void AddRepositories(IServiceCollection services)
        {
            services.AddSingleton<IAdvertisingPlatformRepository, InMemoryAdvertisingPlatformRepository>();
        }

        private static void AddDataStorage(IServiceCollection services)
        {
            
        }
    }
}
