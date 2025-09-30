using AdvertisingPlatforms.Kafka.Abstractions;
using AdvertisingPlatforms.Kafka.Models;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AdvertisingPlatforms.Kafka.HostedServices
{
    public class KafkaSingleMessageConsumerHostedService : KafkaConsumerBase
    {
        private readonly ISingleMessageKafkaConsumerProcessor _processor;

        public KafkaSingleMessageConsumerHostedService(
            ILogger<KafkaSingleMessageConsumerHostedService> logger,
            IOptions<KafkaSettings> options,
            ISingleMessageKafkaConsumerProcessor processor)
            : base(logger, options, options.Value.ComandTopic)
        {
            _processor = processor;
        }

        protected override async Task ConsumeMessagesAsync(IConsumer<string, string> consumer, CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var consumeResult = await ConsumeMessageAsync(consumer, stoppingToken);

                if (consumeResult != null)
                {
                    await _processor.ProcessSingleMessageAsync(consumeResult, stoppingToken);
                }
            }
        }
    }
}
