using AdvertisingPlatforms.Kafka.Models;

namespace AdvertisingPlatforms.Kafka.Abstractions
{
    public interface IKafkaProducer
    {
        Task ProduceAsync(string topic, string key, string value, CancellationToken cancellationToken);
        Task ProduceBatchAsync(string topic, IEnumerable<(string key, string value)> messages, CancellationToken cancellationToken);
    }
}


