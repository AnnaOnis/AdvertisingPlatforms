
using AdvertisingPlatforms.Kafka.Abstractions;
using AdvertisingPlatforms.Kafka.Models;
using Confluent.Kafka;
using DataGenerator.Abstractions;
using Microsoft.Extensions.Logging;

namespace DataGenerator.Services
{
    public class KafkaCommandProcessor : ISingleMessageKafkaConsumerProcessor
    {
        private readonly IGenerationDataService _generator;
        private readonly ILogger<KafkaCommandProcessor> _logger;

        public KafkaCommandProcessor(IGenerationDataService service, 
            ILogger<KafkaCommandProcessor> logger)
        {
            _generator = service;
            _logger = logger;
        }
        public async Task ProcessSingleMessageAsync(ConsumeResult<string, string> message, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Received generation command. Topic: {Topic} Partition: {Partition}, Offset: {Offset}",
                message.Topic, message.Partition, message.Offset);

            await _generator.GenerateAndSendToKafkaAsync(cancellationToken);
        }
    }
}
