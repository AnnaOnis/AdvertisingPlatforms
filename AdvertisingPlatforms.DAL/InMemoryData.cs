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
    public class InMemoryData
    {
        private ImmutableDictionary<string, ImmutableHashSet<AdvertisingPlatform>> _locations =
            ImmutableDictionary<string, ImmutableHashSet<AdvertisingPlatform>>.Empty;

        private readonly ILogger<InMemoryData> _logger;

        public InMemoryData(ILogger<InMemoryData> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Асинхронно сохраняет коллекцию рекламных платформ в памяти
        /// </summary>
        /// <param name="platforms">Коллекция рекламных платформ для сохранения</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <exception cref="ArgumentNullException">Выбрасывается при передаче null коллекции</exception>
        public Task SaveData(IReadOnlyList<AdvertisingPlatform> platforms, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting data upload...");

            if (platforms == null)
            {
                _logger.LogError("Upload failed: null platforms collection");
                throw new ArgumentNullException(nameof(platforms));
            }

            var builder = ImmutableDictionary.CreateBuilder<string, ImmutableHashSet<AdvertisingPlatform>>();

            var processedLocations = 0;

            foreach (var platform in platforms)
            {
                try
                {
                    foreach (var location in platform.Locations)
                    {
                        var prefixes = location.Path.GetPrefixes();

                        foreach (var prefix in prefixes)
                        {
                            if (!builder.TryGetValue(prefix, out var set))
                            {
                                set = ImmutableHashSet<AdvertisingPlatform>.Empty;
                            }
                            set = set.Add(platform);
                            builder[prefix] = set;
                            processedLocations++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing platform {PlatformName}", platform.Name);
                }

            }

            Interlocked.Exchange(ref _locations, builder.ToImmutable());

            _logger.LogInformation("Upload completed. Processed {Platforms} platforms with {Locations} locations",
            platforms.Count, processedLocations);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Получает рекламные платформы по указанной локации
        /// </summary>
        /// <param name="location">Локация для поиска платформ</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Коллекция найденных рекламных платформ</returns>
        /// <exception cref="ArgumentNullException">Выбрасывается при передаче null локации</exception>
        public Task<IReadOnlyList<AdvertisingPlatform>> GetDataByLocation(Location location, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Searching for location: {Location}", location);

            if (location == null)
            {
                _logger.LogWarning("");
                throw new ArgumentNullException(nameof(location));
            }

            if (_locations.TryGetValue(location.Path, out var set))
            {

                _logger.LogInformation("Found {Count} platforms for location {Location}",
                set.Count, location.Path);

                return Task.FromResult<IReadOnlyList<AdvertisingPlatform>>(set.ToList().AsReadOnly());
            }

            return Task.FromResult<IReadOnlyList<AdvertisingPlatform>>(ImmutableList<AdvertisingPlatform>.Empty.AsReadOnly());
        }
    }
}
