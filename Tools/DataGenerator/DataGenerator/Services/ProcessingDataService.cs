using AdvertisingPlatforms.Kafka.Abstractions;
using AdvertisingPlatforms.Kafka.Models;
using DataGenerator.Abstractions;
using DataGenerator.Constants;
using DataGenerator.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace DataGenerator.Services
{
    public class ProcessingDataService : IProcessingDataService
    {
        private readonly JsonSerializerOptions _options; 
        private readonly IKafkaProducer _producer;
        private readonly KafkaSettings _settings;
        private readonly ILogger<ProcessingDataService> _logger;

        public ProcessingDataService(IKafkaProducer kafkaProducer,
            IOptions<KafkaSettings> kafkaSettings,
            ILogger<ProcessingDataService> logger) 
        {
            _options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping            
            };
            _producer = kafkaProducer;
            _settings = kafkaSettings.Value;
            _logger = logger;
        }
        public string SerrializeToJson(List<Advertisement> advertisements)
        {
            string json = JsonSerializer.Serialize(advertisements, _options);

            return json;
        }

        public async Task SaveToFile(string json)
        {
            var filePath = $"{ConfigConstants.DIRECTORI_PATH}/data_{DateTime.Now:dd.MM.yyyy_HHmmss}.json";
            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task SendDataToKafkaAsync(List<Advertisement> advertisements, CancellationToken cancellationToken)
        {
            try
            {
                var messages = advertisements.Select(advertisement => (
                    key: Guid.NewGuid().ToString(),
                    value: JsonSerializer.Serialize(advertisement)
                ));

                await _producer.ProduceBatchAsync(_settings.DataTopic, messages, cancellationToken);

                _logger.LogInformation("Send {Count} records to Kafka", advertisements.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send data");
                throw;
            }
        }
    }
}
