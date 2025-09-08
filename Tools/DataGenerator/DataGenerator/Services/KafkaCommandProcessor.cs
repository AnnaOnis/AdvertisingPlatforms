
using AdvertisingPlatforms.Kafka.Abstractions;
using Confluent.Kafka;
using DataGenerator.Abstractions;
using DataGenerator.Constants;
using Microsoft.Extensions.Logging;

namespace DataGenerator.Services
{
    public class KafkaCommandProcessor : ISingleMessageKafkaConsumerProcessor
    {
        private readonly IGenerationDataService _generator;
        private readonly ILogger<KafkaCommandProcessor> _logger;
        private readonly IProcessingDataService _processingDataService;

        public KafkaCommandProcessor(           
            IGenerationDataService service, 
            ILogger<KafkaCommandProcessor> logger,
            IProcessingDataService processingDataService)
        {
            _generator = service;
            _logger = logger;
            _processingDataService = processingDataService;
        }
        public async Task ProcessSingleMessageAsync(ConsumeResult<string, string> message, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Received generation command. Topic: {Topic} Partition: {Partition}, Offset: {Offset}",
                message.Topic, message.Partition, message.Offset);

            if(int.TryParse(message.Message.Value, out var countData))
            {
                _logger.LogInformation(
                    "Start generate and send data. {time}",
                    DateTime.UtcNow.TimeOfDay);

                var data = _generator.GenerateTestData(countData);
                await _processingDataService.SendDataToKafkaAsync(data, cancellationToken);

                _logger.LogInformation(
                    "Generate and send data compleeted. {time}",
                    DateTime.UtcNow.TimeOfDay);
            }
            else
            {
                _logger.LogWarning("Generate random data count!");

                var random = new Random();
                var randomCount = random.Next(ConfigConstants.MIN_ADS, ConfigConstants.MAX_ADS);
                var data = _generator.GenerateTestData(randomCount);
                await _processingDataService.SendDataToKafkaAsync(data, cancellationToken);
            }
        }
    }
}
