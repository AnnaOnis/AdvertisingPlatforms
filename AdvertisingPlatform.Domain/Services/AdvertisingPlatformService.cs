using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using AdvertisingPlatforms.Domain.Interfaces;
using AdvertisingPlatforms.Domain.Extensions;
using AdvertisingPlatforms.Domain.Entities;
using AdvertisingPlatforms.Domain.Repositories;

namespace AdvertisingPlatforms.Services
{
    public class AdvertisingPlatformService : IAdvertisingPlatformService
    {
        private readonly ILogger<AdvertisingPlatformService> _logger;
        private readonly IValidator<AdvertisingPlatform> _validatorPlatform;
        private readonly IValidator<Location> _validatorLocation;
        private readonly IAdvertisingPlatformRepository _repository;

        public AdvertisingPlatformService(ILogger<AdvertisingPlatformService> logger, 
            IValidator<AdvertisingPlatform> validatorPlatform, 
            IValidator<Location> validatorLocation,
            IAdvertisingPlatformRepository repository)
        {
            _logger = logger;
            _validatorPlatform = validatorPlatform;
            _validatorLocation = validatorLocation;
            _repository = repository;
        }

        public async Task<IReadOnlyList<AdvertisingPlatform>> Search(Location location, CancellationToken cancellationToken)
        {
            _validatorLocation.Validate(location);
            var platforms = await _repository.FindByLocation(location, cancellationToken);
            return platforms;
        }

        public async Task Upload(IReadOnlyList<AdvertisingPlatform> platforms, CancellationToken cancellationToken)
        {
            _validatorPlatform.Validate(platforms);
            foreach ( var platform in platforms)
            {
                _validatorLocation.Validate(platform.Locations);
            }
            await _repository.Save(platforms, cancellationToken);
        }


        //private ImmutableDictionary<string, ImmutableHashSet<AdvertisingPlatform>>  _locations = 
        //    ImmutableDictionary<string, ImmutableHashSet<AdvertisingPlatform>>.Empty;

        //public AdvertisingPlatformService(ILogger<AdvertisingPlatformService> logger)
        //{
        //    _logger = logger;
        //}

        //public async Task Upload(IEnumerable<AdvertisingPlatform> platforms)
        //{
        //    _logger.LogInformation("Starting data upload...");

        //    if (platforms == null)
        //    {
        //        _logger.LogError("Upload failed: null platforms collection");
        //        throw new ArgumentNullException(nameof(platforms));
        //    }

        //    var platformList = platforms.ToList();
        //    if (platformList.Count == 0)
        //    {
        //        _logger.LogWarning("Empty platforms collection uploaded");
        //        return;
        //    }

        //    var builder = ImmutableDictionary.CreateBuilder<string, ImmutableHashSet<AdvertisingPlatform>>();

        //    var processedLocations = 0;

        //    foreach ( var platform in platforms)
        //    {
        //        try
        //        {
        //            foreach ( var location in platform.Locations)
        //            {
        //                var normLocation = location.NormalizeLocation();
        //                if(!builder.TryGetValue(normLocation, out var set))
        //                {
        //                    set = ImmutableHashSet<Platform>.Empty;
        //                }
        //                set = set.Add(platform);
        //                builder[location] = set;
        //                processedLocations++;
        //            }
        //        }
        //        catch(Exception ex)
        //        {
        //            _logger.LogError(ex, "Error processing platform {PlatformName}", platform.Name);
        //        }

        //    }

        //    Interlocked.Exchange(ref _locations, builder.ToImmutable());
        //    _logger.LogInformation("Upload completed. Processed {Platforms} platforms with {Locations} locations",
        //    platformList.Count, processedLocations);
        //}

        //public IReadOnlyList<AdvertisingPlatform> Search (string location)
        //{
        //    _logger.LogDebug("Searching for location: {Location}", location);

        //    if (string.IsNullOrEmpty(location))
        //    {
        //        _logger.LogWarning("Empty location search attempt");
        //        throw new ArgumentNullException(nameof(location));
        //    }

        //    var normLocation = location.NormalizeLocation();
        //    var prefixes = normLocation.GetPrefixes();

        //    _logger.LogDebug("Generated {Count} prefixes for location {Location}", prefixes.Count, normLocation);

        //    var platforms = new HashSet<AdvertisingPlatform>();

        //    foreach ( var prefix in prefixes)
        //    {
        //        if(_locations.TryGetValue(prefix, out var set))
        //        {
        //            platforms.UnionWith(set);
        //        }
        //    }

        //    _logger.LogInformation("Found {Count} platforms for location {Location}",
        //    platforms.Count, location);

        //    return platforms.ToList().AsReadOnly();
        //}
    }
}
