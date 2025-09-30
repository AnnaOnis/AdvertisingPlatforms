using Confluent.Kafka;
using AdvertisingPlatforms.Kafka.Constants;

namespace AdvertisingPlatforms.Kafka.Models
{
    public class KafkaSettings
    {
        public string BootstrapServers { get; set; } = null!;
        public string ComandTopic { get; set; } = null!;
        public string DataTopic { get; set; } = null!;
        public string ConsumerGroupId { get; set; } = null!;
        public string ClientId { get; set; } = null!;
        public string SaslUsername { get; set; } = null!;
        public string SaslPassword { get; set; } = null!;
        public SecurityProtocol? SecurityProtocol { get; set; } = null!;
        public SaslMechanism? SaslMechanism { get; set; } = null!;
        public int MaxBatchSize { get; set; } = KafkaConstants.MAX_BATCH_SIZE;
        public int MaxBatchBytes { get; set; } = KafkaConstants.MAX_BATCH_BYTES;
        public AutoOffsetReset? AutoOffsetReset { get; set; }
        public int PollIntervalMs { get; set; } = KafkaConstants.POLL_INTERVAL_MS;
        public int CommitIntervalMs { get; set; } = KafkaConstants.COMMIT_INTERVAL_MS;
    }
}


