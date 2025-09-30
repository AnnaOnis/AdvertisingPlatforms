using AdvertisingPlatforms.Kafka.Models;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AdvertisingPlatforms.Kafka.HostedServices
{
    public abstract class KafkaConsumerBase : BackgroundService
    {
        protected readonly ILogger<KafkaConsumerBase> Logger;
        protected readonly KafkaSettings Settings;
        protected readonly string Topic;

        public KafkaConsumerBase(ILogger<KafkaConsumerBase> logger, 
            IOptions<KafkaSettings> options, 
            string topic)
        {
            Logger = logger;
            Settings = options.Value;
            Topic = topic;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using IConsumer<string, string> consumer = CreateConsumer();
            consumer.Subscribe(Topic);

            try
            {
                await ConsumeMessagesAsync(consumer, stoppingToken);
            }
            finally
            {
                consumer.Close();
            }
        }

        private IConsumer<string, string> CreateConsumer()
        {
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = Settings.BootstrapServers,
                GroupId = Settings.ConsumerGroupId,
                SecurityProtocol = Settings.SecurityProtocol,
                SaslUsername = Settings.SaslUsername,
                SaslPassword = Settings.SaslPassword,
                SaslMechanism = Settings.SaslMechanism,
                AutoOffsetReset = Settings.AutoOffsetReset,
                EnableAutoCommit = true
            };
            var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
            return consumer;
        }

        protected abstract Task ConsumeMessagesAsync(IConsumer<string, string> consumer, CancellationToken stoppingToken);
        protected async Task<ConsumeResult<string, string>?> ConsumeMessageAsync(IConsumer<string, string> consumer, CancellationToken stoppingToken)
        {
            var consumeResult = default(ConsumeResult<string, string>);
            try
            {
                consumeResult = await Task.Run(() =>
                 consumer.Consume(TimeSpan.FromMilliseconds(100)),
                 stoppingToken);

                return consumeResult;
            }
            catch (ConsumeException ex)
            {
                Logger.LogError(ex, "Kafka consume error: {Reason}", ex.Error.Reason);
                await Task.Delay(3000, stoppingToken);
                return consumeResult;
            }
            catch (OperationCanceledException)
            {
                return consumeResult;
            }
        }
    }
}
