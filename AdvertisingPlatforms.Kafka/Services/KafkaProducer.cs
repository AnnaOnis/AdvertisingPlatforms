using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AdvertisingPlatforms.Kafka.Abstractions;
using AdvertisingPlatforms.Kafka.Models;

namespace AdvertisingPlatforms.Kafka.Services
{
    public class KafkaProducer : IKafkaProducer
    {
        private readonly IProducer<string, string> _producer;
        private readonly ILogger<KafkaProducer> _logger;
        private readonly KafkaSettings _settings;

        public KafkaProducer(IOptions<KafkaSettings> options, ILogger<KafkaProducer> logger)
        {
            _settings = options.Value;
            _logger = logger;
            var config = new ProducerConfig
            {
                BootstrapServers = _settings.BootstrapServers,
                ClientId = _settings.ClientId,
                SecurityProtocol = _settings.SecurityProtocol,
                SaslUsername = _settings.SaslUsername,
                SaslPassword = _settings.SaslPassword,
                SaslMechanism = _settings.SaslMechanism,
                Acks = Acks.All,
                EnableIdempotence = true,
                BatchSize = 64 * 1024,                
            };
            _producer = new ProducerBuilder<string, string>(config).Build();
        }

        public async Task ProduceAsync(string topic, string key, string value, CancellationToken cancellationToken)
        {
            var msg = new Message<string, string> { Key = key, Value = value };
            var dr = await _producer.ProduceAsync(topic, msg, cancellationToken);
            _logger.LogDebug("Produced to {TopicPartitionOffset}", dr.TopicPartitionOffset);
        }

        public async Task ProduceBatchAsync(string topic, IEnumerable<(string key, string value)> messages, CancellationToken cancellationToken)
        {
            foreach (var batch in messages.Chunk(100))
            {
                var tasks = batch.Select(m => _producer.ProduceAsync(topic, new Message<string, string>
                {
                    Key = m.key,
                    Value = m.value
                }, cancellationToken));
                await Task.WhenAll(tasks);
            }
        }
    }
}


