using Confluent.Kafka;

namespace AdvertisingPlatforms.Kafka.Abstractions
{
    public interface IBatchKafkaConsumerProcessor
    {
        Task ProcessBatchAsync(IEnumerable<ConsumeResult<string, string>> batch, CancellationToken cancellationToken);
    }
}


