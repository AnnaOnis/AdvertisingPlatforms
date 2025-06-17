using AdvertisingPlatforms.Domain.Abstractions;
using AdvertisingPlatforms.DAL.Entities;
using AdvertisingPlatforms.Domain.Parser;
using AdvertisingPlatforms.Domain.Services;
using AdvertisingPlatforms.DAL.Abstractions;
using Microsoft.AspNetCore.HttpLogging;
using AdvertisingPlatforms.DAL.Repositories.InMemory;
using AdvertisingPlatforms.DAL;
using AdvertisingPlatforms.DAL.Repositories.DataBase;
using Microsoft.EntityFrameworkCore;
using AdvertisingPlatforms.Domain.Models;
using AdvertisingPlatforms.Domain.Fabrics;
using FluentValidation;
using AdvertisingPlatforms.Web.Validators;
using AdvertisingPlatforms.Web.HttpModels.Requests;
using FluentValidation.AspNetCore;

namespace AdvertisingPlatforms.Web.Configurations
{
    public static class ServiceConfiguration
    {
        public static void ConfigureApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            AddInfrastructure(services);
            AddApplicationComponents(services, config);
            AddFluentValidation(services);
        }

        private static void AddInfrastructure(IServiceCollection services)
        {
            AddLogging(services);
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        private static void AddApplicationComponents(IServiceCollection services, IConfiguration config)
        {
            AddDomainServices(services);
            AddDomainModelFactories(services);
            AddRepositories(services, config);
        }

        private static void AddFluentValidation(IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();
            
            services.AddScoped<IValidator<AdvertisingPlatformRequest>, AdvertisingPlatformRequestValidator>();
            services.AddScoped<IValidator<SearchPlatformsRequest>, SearchPlatformsRequestValidator>();
            services.AddScoped<IValidator<UploadFileRequest>, UploadFileRequestValidator>();
            services.AddScoped<IValidator<LocationRequest>, LocationRequestValidator>();
            services.AddScoped<IValidator<AdvertisementRequest>, AdvertisementRequestValidator>();
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
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<IAdvertisementService, AdvertisementService>();
            services.AddSingleton<IAdvertisingPlatformParser, PlatformsFileParser>();
            services.AddScoped<IUploadDataService, UploadDataService>();
        }

        private static void AddDomainModelFactories(IServiceCollection services)
        {
            services.AddScoped<IDomainModelFactory<AdvertisementDb, Advertisement>, AdvertisementFactory>();
            services.AddScoped<IDomainModelFactory<LocationDb, Location>, LocationFactory>();
            services.AddScoped<IDomainModelFactory<AdvertisingPlatformDb, AdvertisingPlatform>, AdvertisingPlatformFactory>();
        }

        private static void AddRepositories(IServiceCollection services, IConfiguration config)
        {
            var useDatabase = config.GetValue<bool>("UseDatabase");

            if (useDatabase)
            {
                services.AddDbContext<AdvertisingPlatformsDbContext>(options =>
                    options.UseNpgsql(config.GetConnectionString("DefaultConnection")));

                services.AddScoped<IAdvertisingPlatformRepository, EFAdvertisingPlatformRepository>();
                services.AddScoped<IAdvertisementRepository, EFAdvertisementRepository>();
                services.AddScoped<ILocationRepository, EFLocationRepository>();
                services.AddScoped<IUnitOfWork, UnitOfWorkEF>();
            }
            else
            {
                services.AddSingleton<IAdvertisingPlatformRepository, InMemoryAdvertisingPlatformRepository>();
                services.AddSingleton<ILocationRepository, InMemoryLocationRepository>();
                services.AddSingleton<IAdvertisementRepository, InMemoryAdvertisementRepository>();
                services.AddSingleton<IUnitOfWork, UnitOfWorkInMemory>();
            }
            
        }
    }
}
