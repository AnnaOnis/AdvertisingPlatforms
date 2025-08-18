namespace AdvertisingPlatforms.Kafka.Models
{
    public class KafkaSettings
    {
        public string BootstrapServers { get; set; } = null!;
        public string ComandTopic { get; set; } = null!;
        public string DataTopic { get; set; } = null!;
        public string ConsumerGroupId { get; set; } = null!;
        public bool EnableAutoCreateTopics { get; set; } = true;


        // Настройки для батчевой обработки
        public int MaxBatchSize { get; set; } = 100;
        public int MaxBatchBytes { get; set; } = 102400;
        public int PollIntervalMs { get; set; } = 10000;
        public int CommitIntervalMs { get; set; } = 5000;
        //public int ConsumerConcurrency { get; set; } = 1;


        // Настройки для одиночных сообщений
        //public int MaxProcessingTimeMs { get; set; } = 5000;
        //public int RetryDelayMs { get; set; } = 1000;
    }
}


