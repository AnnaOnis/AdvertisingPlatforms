using AdvertisingPlatforms.Kafka.Models;
using Confluent.Kafka;

namespace AdvertisingPlatforms.Kafka.Abstractions
{
    public interface ISingleMessageKafkaConsumerProcessor
    {
        Task ProcessSingleMessageAsync(ConsumeResult<string, string> message, CancellationToken cancellationToken);
    }
}


