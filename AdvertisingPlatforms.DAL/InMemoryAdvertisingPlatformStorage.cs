using System.Collections.Generic;
using System.Collections.Immutable;
using AdvertisingPlatforms.Domain.Entities;
using AdvertisingPlatforms.Domain.Extensions;
using Microsoft.Extensions.Logging;

namespace AdvertisingPlatforms.DAL
{
    /// <summary>
    /// Класс для хранения рекламных платформ в памяти с группировкой по локациям
    /// </summary>
    public class InMemoryAdvertisingPlatformStorage
    {
        private ImmutableDictionary<string, ImmutableHashSet<AdvertisingPlatform>> _platformsByLocationPrefix =
            ImmutableDictionary<string, ImmutableHashSet<AdvertisingPlatform>>.Empty;

        private readonly ILogger<InMemoryAdvertisingPlatformStorage> _logger;

        public InMemoryAdvertisingPlatformStorage(ILogger<InMemoryAdvertisingPlatformStorage> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Cохраняет коллекцию рекламных платформ в памяти
        /// </summary>
        /// <param name="platformsToStore">Коллекция рекламных платформ для сохранения</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <exception cref="ArgumentNullException">Выбрасывается при передаче null коллекции</exception>
        public Task StoreAdvertisingPlatforms(IReadOnlyList<AdvertisingPlatform> platformsToStore, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting data upload...");

            if (platformsToStore == null)
            {
                _logger.LogError("Upload failed: null platforms collection");
                throw new ArgumentNullException(nameof(platformsToStore));
            }

            var platformsByLocationPrefixBuilder = ImmutableDictionary.CreateBuilder<string, ImmutableHashSet<AdvertisingPlatform>>();

            var processedLocations = 0;

            foreach (var platform in platformsToStore)
            {
                try
                {
                    processedLocations = ProcessPlatformLocations(platformsByLocationPrefixBuilder, platform);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing platform {PlatformName}", platform.Name);
                }

            }

            Interlocked.Exchange(ref _platformsByLocationPrefix, platformsByLocationPrefixBuilder.ToImmutable());

            _logger.LogInformation("Upload completed. Processed {PlatformCount} platforms with {LocationPrefixCount} locations",
            platformsToStore.Count, processedLocations);

            return Task.CompletedTask;
        }

        private int ProcessPlatformLocations(ImmutableDictionary<string, ImmutableHashSet<AdvertisingPlatform>>.Builder builder, 
            AdvertisingPlatform platform)
        {
            int processed = 0;

            foreach (var location in platform.Locations)
            {

                processed += AddPlatformToLocationPrefixes(builder, platform, location);
            }

            return processed;
        }

        private int AddPlatformToLocationPrefixes(ImmutableDictionary<string, ImmutableHashSet<AdvertisingPlatform>>.Builder builder, 
            AdvertisingPlatform platform, 
            Location location)
        {
            int processed = 0; 

            var prefixes = location.Path.GetPrefixes();

            foreach (var prefix in prefixes)
            {
                if (!builder.TryGetValue(prefix, out var set))
                {
                    set = ImmutableHashSet<AdvertisingPlatform>.Empty;
                }
                set = set.Add(platform);
                builder[prefix] = set;
                processed++;
            }

            return processed;
        }

        /// <summary>
        /// Получает рекламные платформы по указанной локации
        /// </summary>
        /// <param name="targetLocation">Локация для поиска платформ</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Коллекция найденных рекламных платформ</returns>
        /// <exception cref="ArgumentNullException">Выбрасывается при передаче null локации</exception>
        public Task<IReadOnlyCollection<AdvertisingPlatform>> FindPlatformsByLocation(Location targetLocation, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Searching for location: {Location}", targetLocation);

            if (targetLocation == null)
            {
                _logger.LogWarning("");
                throw new ArgumentNullException(nameof(targetLocation));
            }

            if (_platformsByLocationPrefix.TryGetValue(targetLocation.Path, out var setPlatforms))
            {

                _logger.LogInformation("Found {Count} platforms for location {Location}",
                setPlatforms.Count, targetLocation.Path);

                return Task.FromResult<IReadOnlyCollection<AdvertisingPlatform>>(setPlatforms);
            }

            return Task.FromResult<IReadOnlyCollection<AdvertisingPlatform>>(ImmutableList<AdvertisingPlatform>.Empty);
        }


    }
}
