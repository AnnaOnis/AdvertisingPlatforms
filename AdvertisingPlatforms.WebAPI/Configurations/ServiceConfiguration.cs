using AdvertisingPlatforms.Domain.Abstractions;
using AdvertisingPlatforms.DAL.Entities;
using AdvertisingPlatforms.Domain.Parser;
using AdvertisingPlatforms.Domain.Services;
using AdvertisingPlatforms.DAL.Abstractions;
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
using AdvertisingPlatforms.Domain.Parsers;
using AdvertisingPlatforms.Kafka.Extensions;
using AdvertisingPlatforms.Kafka.HostedServices;
using AdvertisingPlatforms.Kafka.Abstractions;
using AdvertisingPlatforms.Web.Services;

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
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        private static void AddApplicationComponents(IServiceCollection services, IConfiguration config)
        {
            AddDomainServices(services);
            AddDomainModelFactories(services);
            AddRepositories(services, config);
            services.AddAutoMapper(typeof(Program).Assembly);
            services.AddKafka(config);
            services.AddScoped<IBatchKafkaConsumerProcessor, KafkaUploadProcessor>();

            services.AddHostedService<KafkaBatchConsumerHostedService>();
        }

        private static void AddFluentValidation(IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();
            
            services.AddScoped<IValidator<CreateAdvertisingPlatformRequest>, CreateAdvertisingPlatformRequestValidator>();
            services.AddScoped<IValidator<UpdateAdvertisingPlatformRequest>, UpdateAdvertisingPlatformRequestValidator>();
            services.AddScoped<IValidator<SearchPlatformsRequest>, SearchPlatformsRequestValidator>();
            services.AddScoped<IValidator<UploadFileRequest>, UploadFileRequestValidator>();
            services.AddScoped<IValidator<CreateLocationRequest>, CreateLocationRequestValidator>();
            services.AddScoped<IValidator<UpdateLocationRequest>, UpdateLocationRequestValidator>();
            services.AddScoped<IValidator<CreateAdvertisementRequest>, CreateAdvertisementRequestValidator>();
            services.AddScoped<IValidator<UpdateAdvertisementRequest>, UpdateAdvertisementRequestValidator>();
        }

        private static void AddDomainServices(IServiceCollection services)
        {       
            services.AddScoped<IAdvertisingPlatformService, AdvertisingPlatformService>();
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<IAdvertisementService, AdvertisementService>();
            services.AddScoped<IUploadDataService, UploadDataService>();
            services.AddSingleton<IAdvertisingPlatformParser, PlatformsFileParser>();
            services.AddSingleton<IFileParseStrategy, JsonFileParseStrategy>();
            services.AddSingleton<IFileParseStrategy, TextFileParseStrategy>();
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
                var connectionString = config.GetConnectionString("DefaultConnection") 
                    ?? Environment.GetEnvironmentVariable("CONNECTION_STRING");

                services.AddDbContext<AdvertisingPlatformsDbContext>(options =>
                    options.UseNpgsql(connectionString));

                services.AddScoped<IAdvertisingPlatformRepository, EFAdvertisingPlatformRepository>();
                services.AddScoped<IAdvertisementRepository, EFAdvertisementRepository>();
                services.AddScoped<ILocationRepository, EFLocationRepository>();
                services.AddScoped<IUploadErrorRepository, EFUploadErrorRepository>();
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
