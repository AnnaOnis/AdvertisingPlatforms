using AdvertisingPlatforms.Base.Extensions;
using AdvertisingPlatforms.Base.Constants;
using AdvertisingPlatforms.DAL.Abstractions;
using AdvertisingPlatforms.DAL.Entities;
using AdvertisingPlatforms.Domain.Abstractions;
using AdvertisingPlatforms.Domain.DTOs;
using Microsoft.Extensions.Logging;
using AdvertisingPlatforms.Base.Exceptions;

namespace AdvertisingPlatforms.Domain.Services
{
    public class UploadDataService : IUploadDataService
    {
        private readonly IAdvertisingPlatformParser _fileParser;
        private readonly ILogger<UploadDataService> _logger;
        private readonly IAdvertisementRepository _advertisementRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IAdvertisingPlatformRepository _advertisingPlatformRepository;

        public UploadDataService(
            IAdvertisingPlatformParser fileParser,
            ILogger<UploadDataService> logger,
            IAdvertisementRepository advertisementRepository,
            ILocationRepository locationRepository,
            IAdvertisingPlatformRepository advertisingPlatformRepository)
        {
            _fileParser = fileParser;
            _logger = logger;
            _advertisementRepository = advertisementRepository;
            _locationRepository = locationRepository;
            _advertisingPlatformRepository = advertisingPlatformRepository;
        }

        public async Task UploadDataFromFile(IFileData fileData, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LogMessages.STARTING_FILE_UPLOAD, fileData.FileName);
            
            using var stream = await fileData.FileToMemoryStreamAsync(cancellationToken);

            _logger.LogInformation(LogMessages.PARSING_FILE_CONTENT);
            var parseDataItems = _fileParser.ParseFile(stream);

            if (parseDataItems == null || parseDataItems.Count == 0)
            {
                throw new DomainValidationException(ErrorMessages.NO_DATA_TO_DOWNLOAD);
            }

            var locationDictionary = await CreateLocations(parseDataItems, cancellationToken);
            var advertisementDictionary = await CreateAdvertisements(parseDataItems, cancellationToken);
            await CreateAdvettisingPlatform(parseDataItems, locationDictionary, advertisementDictionary, cancellationToken);
            
            _logger.LogInformation(LogMessages.DATA_UPLOADED_SUCCESSFULLY);
        }

        private async Task<Dictionary<string, LocationDb>> CreateLocations(IReadOnlyCollection<ParseDataDto> parseData, CancellationToken cancellationToken)
        {
            var locationDictionary = new Dictionary<string, LocationDb>();
            var addedCount = 0;
            var skippedCount = 0;

            var allPaths = parseData.SelectMany(d => d.LocationPaths)
                               .Select(p => p.NormalizeLocationPath())
                               .Distinct()
                               .ToList();

            var sortedPaths = allPaths.OrderBy(p => p.Split(TextSeparators.SLASH).Length).ToList();

            foreach (var path in sortedPaths)
            {
                if (locationDictionary.ContainsKey(path)) continue;

                var existingLocation = await _locationRepository.FindByPath(path, cancellationToken);
                if (existingLocation != null)
                {
                    locationDictionary.Add(path, existingLocation);
                    _logger.LogInformation(LogMessages.LOCATION_ALREADY_EXISTS, path);
                    skippedCount++;
                    continue;
                }

                var parent = await FindParentLocation(path, locationDictionary, cancellationToken);
                var location = new LocationDb(path, parent?.Id);

                locationDictionary.Add(path, location);
                await _locationRepository.Add(location, cancellationToken);
                _logger.LogInformation(LogMessages.LOCATION_CREATED, path);
                addedCount++;
            }

            foreach (var path in sortedPaths)
            {
                var location = locationDictionary[path];
                if (location.Parent != null)
                {
                    location.Parent.Children.Add(location);
                }
            }

            _logger.LogInformation(LogMessages.LOCATIONS_UPLOAD_SUMMARY, addedCount, skippedCount);
            return locationDictionary;
        }

        private async Task<LocationDb?> FindParentLocation(string path, Dictionary<string, LocationDb> locations, CancellationToken cancellationToken)
        {
            var segments = path.Split(TextSeparators.SLASH, StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length <= 1) return null;

            var parentPath = string.Join(TextSeparators.SLASH, segments.Take(segments.Length - 1));
            parentPath = parentPath.NormalizeLocationPath();

            if (locations.TryGetValue(parentPath, out var parent))
                return parent;

            var existingParent = await _locationRepository.FindByPath(parentPath, cancellationToken);
            if (existingParent != null)
            {
                locations.Add(parentPath, existingParent);
                _logger.LogInformation(LogMessages.LOCATION_ALREADY_EXISTS, parentPath);
                return existingParent;
            }

            var newParent = await FindParentLocation(parentPath, locations, cancellationToken);
            
            if (newParent == null)
            {
                newParent = new LocationDb(parentPath, null);
                locations.Add(parentPath, newParent);
                await _locationRepository.Add(newParent, cancellationToken);
                _logger.LogInformation(LogMessages.LOCATION_CREATED, parentPath);
            }

            return newParent;
        }

        private async Task<Dictionary<string, AdvertisementDb>> CreateAdvertisements(IReadOnlyCollection<ParseDataDto> parseData,
            CancellationToken cancellationToken)
        {
            var advertisementDictionary = new Dictionary<string, AdvertisementDb>();
            var uniqueNames = parseData.Select(d => d.AdvertisementName).Distinct();
            var addedCount = 0;
            var skippedCount = 0;

            foreach (var name in uniqueNames)
            {
                var existingAdvertisement = await _advertisementRepository.FindByName(name, cancellationToken);
                if (existingAdvertisement != null)
                {
                    advertisementDictionary.Add(name, existingAdvertisement);
                    _logger.LogInformation(LogMessages.ADVERTISEMENT_ALREADY_EXISTS, name);
                    skippedCount++;
                    continue;
                }

                var advertisement = new AdvertisementDb(name);
                advertisementDictionary.Add(advertisement.Name, advertisement);
                await _advertisementRepository.Add(advertisement, cancellationToken);
                _logger.LogInformation(LogMessages.ADVERTISEMENT_CREATED, name);
                addedCount++;
            }

            _logger.LogInformation(LogMessages.ADVERTISEMENTS_UPLOAD_SUMMARY, addedCount, skippedCount);
            return advertisementDictionary;
        }

        private async Task CreateAdvettisingPlatform(IReadOnlyCollection<ParseDataDto> parseData, 
            Dictionary<string, LocationDb> locations,
            Dictionary<string, AdvertisementDb> advertisements,
            CancellationToken cancellationToken)
        {
            var skippedCount = 0;
            var addedCount = 0;

            foreach (var dto in parseData)
            {
                foreach (var path in dto.LocationPaths)
                {
                    var normalizedPath = path.NormalizeLocationPath();
                    
                    if (!locations.TryGetValue(normalizedPath, out var location))
                    {
                        throw new InvalidOperationException($"Location '{normalizedPath}' not found");
                    }

                    if (!advertisements.TryGetValue(dto.AdvertisementName, out var advertisement))
                    {
                        throw new InvalidOperationException($"Advertisement '{dto.AdvertisementName}' not found");
                    }

                    var exists = await _advertisingPlatformRepository.ExistsByAdvertisementAndLocation(
                        advertisement.Id, location.Id, cancellationToken);

                    if (exists)
                    {
                        _logger.LogInformation(LogMessages.PLATFORM_ALREADY_EXISTS, 
                            dto.AdvertisementName, normalizedPath);
                        skippedCount++;
                        continue;
                    }

                    var platform = new AdvertisingPlatformDb(advertisement.Id, location.Id)
                    {
                        Advertisement = advertisement,
                        Location = location
                    };

                    await _advertisingPlatformRepository.Add(platform, cancellationToken);
                    _logger.LogInformation(LogMessages.PLATFORM_CREATED, 
                        dto.AdvertisementName, normalizedPath);
                    addedCount++;
                }
            }

            _logger.LogInformation(LogMessages.PLATFORMS_UPLOAD_SUMMARY, addedCount, skippedCount);
        }
    }
}
