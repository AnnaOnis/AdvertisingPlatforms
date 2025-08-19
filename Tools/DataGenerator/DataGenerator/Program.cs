using AdvertisingPlatforms.Kafka.Abstractions;
using AdvertisingPlatforms.Kafka.Extensions;
using AdvertisingPlatforms.Kafka.HostedServices;
using DataGenerator.Abstractions;
using DataGenerator.HostedServices;
using DataGenerator.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


var builder = Host.CreateApplicationBuilder(args);

// Конфигурация
builder.Configuration.AddJsonFile("appsettings.json");

// Регистрация сервисов Kafka
builder.Services.AddKafka(builder.Configuration);

// Сервисы приложения
builder.Services.AddSingleton<IGenerationDataService, GenerationDataService>();
builder.Services.AddSingleton<IProcessingDataService, ProcessingDataService>();
builder.Services.AddSingleton<ISingleMessageKafkaConsumerProcessor, KafkaCommandProcessor>();

// Фоновые сервисы
builder.Services.AddHostedService<ScheduledGenerationService>();
builder.Services.AddHostedService<KafkaSingleMessageConsumerHostedService>();

// Запуск
await builder.Build().RunAsync();


