using Microsoft.Extensions.Logging;
using AdvertisingPlatforms.Domain.Abstractions;
using AdvertisingPlatforms.DAL.Entities;
using AdvertisingPlatforms.DAL.Abstractions;

namespace AdvertisingPlatforms.Domain.Services
{
    public class AdvertisingPlatformService : IAdvertisingPlatformService
    {
        private readonly ILogger<AdvertisingPlatformService> _logger;
        private readonly IValidator<AdvertisingPlatform> _validatorPlatform;
        private readonly IValidator<Location> _validatorLocation;
        private readonly IFileDataValidator<IFileData> _fileDataValidator;
        private readonly IAdvertisingPlatformRepository _platformRepository;
        private readonly IAdvertisingPlatformParser _advertisingPlatformParser;

        public AdvertisingPlatformService(ILogger<AdvertisingPlatformService> logger, 
            IValidator<AdvertisingPlatform> validatorPlatform, 
            IValidator<Location> validatorLocation,
            IFileDataValidator<IFileData> fileDataValidator,
            IAdvertisingPlatformRepository repository,
            IAdvertisingPlatformParser advertisingPlatformParser)
        {
            _logger = logger;
            _validatorPlatform = validatorPlatform;
            _validatorLocation = validatorLocation;
            _fileDataValidator = fileDataValidator;
            _platformRepository = repository;
            _advertisingPlatformParser = advertisingPlatformParser;
        }

        public async Task<IReadOnlyCollection<AdvertisingPlatform>> Search(string locationPath, CancellationToken cancellationToken)
        {
            var location = new Location(locationPath);

            _validatorLocation.Validate(location);

            var platforms = await _platformRepository.FindByLocationAsync(location, cancellationToken);
            return platforms;
        }

        public async Task Upload(IReadOnlyList<AdvertisingPlatform> platforms, CancellationToken cancellationToken)
        {
            _validatorPlatform.Validate(platforms);
            foreach ( var platform in platforms)
            {
                _validatorLocation.Validate(platform.Locations);
            }
            await _platformRepository.AddRangeAsync(platforms, cancellationToken);
        }

        public async Task<int> UploadFromFile(IFileData file, CancellationToken cancellationToken)
        {
            _fileDataValidator.Validate(file);
            var stream = await file.FileToMemoryStreamAsync(cancellationToken);
            var platforms = _advertisingPlatformParser.ParseFile(stream);

            await Upload(platforms, cancellationToken);

            return platforms.Count;
        }
    }
}
