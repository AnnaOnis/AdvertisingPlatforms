using Microsoft.Extensions.Logging;
using AdvertisingPlatforms.Domain.Abstractions;
using AdvertisingPlatforms.DAL.Entities;
using AdvertisingPlatforms.DAL.Abstractions;
using AdvertisingPlatforms.Base.Constants;

namespace AdvertisingPlatforms.Domain.Services
{
    public class AdvertisingPlatformService : IAdvertisingPlatformService
    {
        private readonly ILogger<AdvertisingPlatformService> _logger;
        private readonly IValidator<AdvertisingPlatform> _validatorPlatform;
        private readonly IValidator<Location> _validatorLocation;
        private readonly IAdvertisingPlatformRepository _platformRepository;
        private readonly IAdvertisingPlatformParser _advertisingPlatformParser;

        public AdvertisingPlatformService(ILogger<AdvertisingPlatformService> logger, 
            IValidator<AdvertisingPlatform> validatorPlatform, 
            IValidator<Location> validatorLocation,
            IAdvertisingPlatformRepository repository,
            IAdvertisingPlatformParser advertisingPlatformParser)
        {
            _logger = logger;
            _validatorPlatform = validatorPlatform;
            _validatorLocation = validatorLocation;
            _platformRepository = repository;
            _advertisingPlatformParser = advertisingPlatformParser;
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

        public async Task<int> UploadFromStream(Stream stream, CancellationToken cancellationToken)
        {
            var platforms = _advertisingPlatformParser.ParseFile(stream);

            await Upload(platforms, cancellationToken);

            return platforms.Count;
        }
    }
}
