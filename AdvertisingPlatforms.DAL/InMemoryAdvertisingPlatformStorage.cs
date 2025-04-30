using System.Collections.Generic;
using System.Collections.Immutable;
using AdvertisingPlatforms.Domain.Entities;
using AdvertisingPlatforms.Domain.Extensions;
using Microsoft.Extensions.Logging;

namespace AdvertisingPlatforms.DAL
{
    /// <summary>
    /// In-memory storage for advertising platforms with location-based grouping
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
        /// Stores a collection of advertising platforms in memory
        /// </summary>
        /// <param name="platformsToStore">Collection of advertising platforms to store</param>
        /// <param name="cancellationToken">Operation cancellation token</param>
        /// <exception cref="ArgumentNullException">Thrown when null collection is provided</exception>
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
        /// Retrieves advertising platforms by specified location
        /// </summary>
        /// <param name="targetLocation">Location to search platforms for</param>
        /// <param name="cancellationToken">Operation cancellation token</param>
        /// <returns>Collection of found advertising platforms</returns>
        /// <exception cref="ArgumentNullException">Thrown when null location is provided</exception>
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
