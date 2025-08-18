using AdvertisingPlatforms.Kafka.Abstractions;
using AdvertisingPlatforms.Kafka.Models;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AdvertisingPlatforms.Kafka.HostedServices
{
    public class KafkaSingleMessageConsumerHostedService : BackgroundService
    {
        private readonly ILogger<KafkaSingleMessageConsumerHostedService> _logger;
        private readonly KafkaSettings _settings;
        private readonly ISingleMessageKafkaConsumerProcessor _processor;

        public KafkaSingleMessageConsumerHostedService(
            IOptions<KafkaSettings> options,
            ISingleMessageKafkaConsumerProcessor processor,
            ILogger<KafkaSingleMessageConsumerHostedService> logger)
        {
            _logger = logger;
            _settings = options.Value;
            _processor = processor;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = _settings.BootstrapServers,
                GroupId = _settings.ConsumerGroupId,
                EnableAutoCommit = true,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                SessionTimeoutMs = 6000,
                MaxPollIntervalMs = _settings.PollIntervalMs,
                AllowAutoCreateTopics = _settings.EnableAutoCreateTopics
            };

            using var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
            consumer.Subscribe(_settings.ComandTopic);

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = await Task.Run(() =>
                         consumer.Consume(TimeSpan.FromMilliseconds(_settings.PollIntervalMs)),
                         stoppingToken);

                        if (consumeResult == null) continue;

                        await _processor.ProcessSingleMessageAsync(consumeResult, stoppingToken);
                    }
                    catch (ConsumeException ex)
                    {
                        _logger.LogError(ex, "Kafka consume error: {Reason}", ex.Error.Reason);
                        await Task.Delay(1000, stoppingToken);
                    }
                    catch (OperationCanceledException)
                    {
                        // Игнорируем отмену
                    }
                }
            }
            finally
            {
                consumer.Close();
            }
        }
    }
}
