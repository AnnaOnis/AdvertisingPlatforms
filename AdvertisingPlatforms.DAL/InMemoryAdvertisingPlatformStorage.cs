using System.Collections.Immutable;
using AdvertisingPlatforms.Base.Extensions;
using AdvertisingPlatforms.DAL.Entities;
using Microsoft.Extensions.Logging;
using AdvertisingPlatforms.Base.Constants;

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
            if (platformsToStore == null)
            {
                _logger.LogError(ErrorMessages.UPLOAD_DATA_FAILED + ErrorMessages.NULL_PLATFORMS_COLLECTION);
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
                    _logger.LogError(ex, ErrorMessages.ERROR_PROCESSING_PLATFORM, platform.Advertisement.Name);
                }

            }

            Interlocked.Exchange(ref _platformsByLocationPrefix, platformsByLocationPrefixBuilder.ToImmutable());

            _logger.LogInformation(LogMessages.DATA_UPLOADED_SUCCESSFULLY + LogMessages.COUNT_PROCESSED_LOCATIONS,
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
            _logger.LogDebug(LogMessages.SEARCH_REQUEST_FOR_LOCATION, targetLocation);

            if (targetLocation == null)
            {
                _logger.LogWarning(ErrorMessages.NULL_LOCATION);
                throw new ArgumentNullException(nameof(targetLocation));
            }

            if (_platformsByLocationPrefix.TryGetValue(targetLocation.Path, out var setPlatforms))
            {

                _logger.LogInformation(LogMessages.RETURNING_PLATFORMS_FOR_LOCATION,
                setPlatforms.Count, targetLocation.Path);

                return Task.FromResult<IReadOnlyCollection<AdvertisingPlatform>>(setPlatforms);
            }

            return Task.FromResult<IReadOnlyCollection<AdvertisingPlatform>>(ImmutableList<AdvertisingPlatform>.Empty);
        }


    }
}
