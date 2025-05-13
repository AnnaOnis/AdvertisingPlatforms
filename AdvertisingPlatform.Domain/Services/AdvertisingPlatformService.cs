using Microsoft.Extensions.Logging;
using AdvertisingPlatforms.Domain.Interfaces;
using AdvertisingPlatforms.Domain.Entities;
using AdvertisingPlatforms.Domain.Abstractions;

namespace AdvertisingPlatforms.Services
{
    public class AdvertisingPlatformService : IAdvertisingPlatformService
    {
        private readonly ILogger<AdvertisingPlatformService> _logger;
        private readonly IValidator<AdvertisingPlatform> _validatorPlatform;
        private readonly IValidator<Location> _validatorLocation;
        private readonly IAdvertisingPlatformRepository _platformRepository;

        public AdvertisingPlatformService(ILogger<AdvertisingPlatformService> logger, 
            IValidator<AdvertisingPlatform> validatorPlatform, 
            IValidator<Location> validatorLocation,
            IAdvertisingPlatformRepository repository)
        {
            _logger = logger;
            _validatorPlatform = validatorPlatform;
            _validatorLocation = validatorLocation;
            _platformRepository = repository;
        }

        public async Task<IReadOnlyCollection<AdvertisingPlatform>> Search(Location location, CancellationToken cancellationToken)
        {
            _validatorLocation.Validate(location);
            var platforms = await _platformRepository.FindByLocation(location, cancellationToken);
            return platforms;
        }

        public async Task Upload(IReadOnlyList<AdvertisingPlatform> platforms, CancellationToken cancellationToken)
        {
            _validatorPlatform.Validate(platforms);
            foreach ( var platform in platforms)
            {
                _validatorLocation.Validate(platform.Locations);
            }
            await _platformRepository.Save(platforms, cancellationToken);
        }
    }
}
