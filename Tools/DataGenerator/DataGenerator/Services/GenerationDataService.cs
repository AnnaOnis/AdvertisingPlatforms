using DataGenerator.Abstractions;
using DataGenerator.Constants;
using DataGenerator.Models;
using Microsoft.Extensions.Logging;

namespace DataGenerator.Services
{
    public class GenerationDataService : IGenerationDataService
    {
        private readonly ILogger<GenerationDataService> _logger;

        public GenerationDataService(
            ILogger<GenerationDataService> logger)
        {
            _logger = logger;
        }

        public List<Advertisement> GenerateTestData(int count)
        {
            var locationHierarchy = GenerateLocationHierarchy();
            var advertisements = new List<Advertisement>();

            for (int i = 0; i < count; i++)
            {
                advertisements.Add(new Advertisement(GenerateAdvertisementName(), SelectLocations(locationHierarchy)));
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

        private List<string> SelectLocations(List<string> allLocations)
        {
            var random = new Random();
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

        private string GenerateAdvertisementName()
        {
            var random = new Random();
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
