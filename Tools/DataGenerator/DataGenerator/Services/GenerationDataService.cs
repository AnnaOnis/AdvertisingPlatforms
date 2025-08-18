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
    public class GenerationDataService : IGenerationDataService
    {
        private readonly IKafkaProducer _producer;
        private readonly KafkaSettings _settings;
        private readonly ILogger<GenerationDataService> _logger;

        public GenerationDataService(IKafkaProducer kafkaProducer,
            IOptions<KafkaSettings> options,
            ILogger<GenerationDataService> logger)
        {
            _producer = kafkaProducer;
            _settings = options.Value;
            _logger = logger;
        }

        public async Task GenerateAndSendToKafkaAsync(CancellationToken cancellationToken)
        {
            try
            {
                // 1. Генерация данных
                var random = new Random();
                var count = random.Next(ConfigConstants.MIN_ADS, ConfigConstants.MAX_ADS + 1);

                var data = GenerateTestData(count);               

                // 2. Подготовка сообщений
                var messages = data.Select(d => (
                    key: Guid.NewGuid().ToString(),
                    value: JsonSerializer.Serialize(d)
                ));

                // 3. Отправка в Kafka
                await _producer.ProduceBatchAsync(_settings.DataTopic, messages, cancellationToken);

                _logger.LogInformation("Sent {Count} records to Kafka", data.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate and send data");
                throw;
            }
        }
        public List<Advertisement> GenerateTestData(int count)
        {
            var random = new Random();
            var locationHierarchy = GenerateLocationHierarchy();
            var advertisements = new List<Advertisement>();

            for (int i = 0; i < count; i++)
            {
                advertisements.Add(new Advertisement(GenerateAdvertisementName(random), SelectLocations(locationHierarchy, random)));
            }

            return advertisements;
        }
        private List<string> GenerateLocationHierarchy()
        {
            var locations = new List<string> { LocationPathConstants.ROOT_LOCATION };

            foreach (var region in LocationPathConstants.REGIONS)
            {
                string regionPath = $"{LocationPathConstants.ROOT_LOCATION}/{region.Key}";
                locations.Add(regionPath);

                foreach (var city in region.Value)
                {
                    locations.Add($"{regionPath}/{city}");
                }
            }

            return locations;
        }

        private List<string> SelectLocations(List<string> allLocations, Random random)
        {
            int count = random.Next(1, 6);
            var selected = new List<string>();

            while (selected.Count < count)
            {
                var location = allLocations[random.Next(allLocations.Count)];
                if (!selected.Contains(location))
                {
                    selected.Add(location);
                }
            }

            return selected;
        }

        private string GenerateAdvertisementName(Random random)
        {
            int pattern = random.Next(5);
            return pattern switch
            {
                0 => $"{AdvertisementNameConstants.NAME_PREFIXES[random.Next(AdvertisementNameConstants.NAME_PREFIXES.Length)]}" +
                        $".{AdvertisementNameConstants.NAME_SUFFIXES[random.Next(AdvertisementNameConstants.NAME_SUFFIXES.Length)]}",
                1 => $"{AdvertisementNameConstants.NAME_TOPICS[random.Next(AdvertisementNameConstants.NAME_TOPICS.Length)]}" +
                        $" {AdvertisementNameConstants.NAME_SUFFIXES[random.Next(AdvertisementNameConstants.NAME_SUFFIXES.Length)]}",
                2 => $"{AdvertisementNameConstants.NAME_PREFIXES[random.Next(AdvertisementNameConstants.NAME_PREFIXES.Length)]}" +
                        $" {AdvertisementNameConstants.NAME_TOPICS[random.Next(AdvertisementNameConstants.NAME_TOPICS.Length)]}",
                3 => $"{AdvertisementNameConstants.NAME_ADJECTIVES[random.Next(AdvertisementNameConstants.NAME_ADJECTIVES.Length)]}" +
                        $" {AdvertisementNameConstants.NAME_NOUNS[random.Next(AdvertisementNameConstants.NAME_NOUNS.Length)]}",
                _ => $"{AdvertisementNameConstants.NAME_TOPICS[random.Next(AdvertisementNameConstants.NAME_TOPICS.Length)]} " +
                        $"{AdvertisementNameConstants.NAME_ADJECTIVES[random.Next(AdvertisementNameConstants.NAME_ADJECTIVES.Length)]}"
            };
        }
    }
}
