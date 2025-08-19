using Confluent.Kafka;

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
        public int MaxBatchSize { get; set; } = 100;
        public int MaxBatchBytes { get; set; } = 102400;
        public AutoOffsetReset? AutoOffsetReset { get; set; }
        public int PollIntervalMs { get; set; } = 10000;
        public int CommitIntervalMs { get; set; } = 5000;
    }
}


