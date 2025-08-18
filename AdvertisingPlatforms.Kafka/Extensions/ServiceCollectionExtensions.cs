using AdvertisingPlatforms.Kafka.Abstractions;
using AdvertisingPlatforms.Kafka.HostedServices;
using AdvertisingPlatforms.Kafka.Models;
using AdvertisingPlatforms.Kafka.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AdvertisingPlatforms.Kafka.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddKafka(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<KafkaSettings>(configuration.GetSection("Kafka"));
            services.AddSingleton<IKafkaProducer, KafkaProducer>();
            return services;
        }
    }
}


