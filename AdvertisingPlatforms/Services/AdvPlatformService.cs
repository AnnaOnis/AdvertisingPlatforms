using System.Collections.Immutable;
using System.Runtime.InteropServices;
using System.Text;
using AdvertisingPlatforms.Models;

namespace AdvertisingPlatforms.Services
{
    /// <summary>
    /// Сервис для работы с рекламными площадками
    /// </summary>
    public class AdvPlatformService
    {
        private readonly ILogger<AdvPlatformService> _logger;
        private ImmutableDictionary<string, ImmutableHashSet<AdvPlatform>>  _locations = 
            ImmutableDictionary<string, ImmutableHashSet<AdvPlatform>>.Empty;

        public AdvPlatformService(ILogger<AdvPlatformService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Загружает новые данные о площадках
        /// </summary>
        /// <param name="platforms">Коллекция рекламных площадок</param>
        public void Upload(IEnumerable<AdvPlatform> platforms)
        {
            _logger.LogInformation("Starting data upload...");

            if (platforms == null)
            {
                _logger.LogError("Upload failed: null platforms collection");
                throw new ArgumentNullException(nameof(platforms));
            }

            var platformList = platforms.ToList();
            if (platformList.Count == 0)
            {
                _logger.LogWarning("Empty platforms collection uploaded");
                return;
            }

            var builder = ImmutableDictionary.CreateBuilder<string, ImmutableHashSet<AdvPlatform>>();

            var processedLocations = 0;

            foreach ( var platform in platforms)
            {
                try
                {
                    foreach ( var location in platform.Locations)
                    {
                        var normLocation = NormalizeLocation(location);
                        if(!builder.TryGetValue(normLocation, out var set))
                        {
                            set = ImmutableHashSet<AdvPlatform>.Empty;
                        }
                        set = set.Add(platform);
                        builder[location] = set;
                        processedLocations++;
                    }
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "Error processing platform {PlatformName}", platform.Name);
                }

            }

            Interlocked.Exchange(ref _locations, builder.ToImmutable());
            _logger.LogInformation("Upload completed. Processed {Platforms} platforms with {Locations} locations",
            platformList.Count, processedLocations);
        }

        /// <summary>
        /// Ищет площадки по указанной локации
        /// </summary>
        /// <param name="location">Целевая локация</param>
        /// <returns>Коллекция подходящих площадок</returns>
        public IEnumerable<AdvPlatform> Search (string location)
        {
            _logger.LogDebug("Searching for location: {Location}", location);

            if (string.IsNullOrEmpty(location))
            {
                _logger.LogWarning("Empty location search attempt");
                throw new ArgumentNullException(nameof(location));
            }

            var normLocation = NormalizeLocation(location);
            var prefixes = GetPrefixes(normLocation);
            var platforms = new HashSet<AdvPlatform>();

            foreach ( var prefix in prefixes)
            {
                if(_locations.TryGetValue(prefix, out var set))
                {
                    platforms.UnionWith(set);
                }
            }

            _logger.LogInformation("Found {Count} platforms for location {Location}",
            platforms.Count, location);

            return platforms;
        }

        private string NormalizeLocation(string location)
        {
            if (string.IsNullOrEmpty(location)) return "/";

            var trimmed = location.Trim();
            if (!trimmed.StartsWith("/")) trimmed = "/" + trimmed;
            return trimmed.TrimEnd('/');
        }
        private IEnumerable<string> GetPrefixes(string location)
        {
            if (string.IsNullOrEmpty(location)) throw new ArgumentNullException(nameof(location));

            var parts = location.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            var prefixes = new List<string>();
            var sb = new StringBuilder();

            foreach (var part in parts)
            {
                sb.Append('/');
                sb.Append(part);
                prefixes.Add(sb.ToString());
            }

            _logger.LogDebug("Generated {Count} prefixes for location {Location}",
            prefixes.Count, location);

            return prefixes;
        }
    }
}
