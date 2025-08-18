using AdvertisingPlatforms.Base.Extensions;
using AdvertisingPlatforms.Base.Constants;
using AdvertisingPlatforms.DAL.Abstractions;
using AdvertisingPlatforms.DAL.Entities;
using AdvertisingPlatforms.Domain.Abstractions;
using AdvertisingPlatforms.Domain.DTOs;
using Microsoft.Extensions.Logging;

namespace AdvertisingPlatforms.Domain.Services
{
    public class UploadDataService : IUploadDataService
    {
        private readonly IAdvertisingPlatformParser _fileParser;
        private readonly ILogger<UploadDataService> _logger;
        private readonly IAdvertisementRepository _advertisementRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IAdvertisingPlatformRepository _advertisingPlatformRepository;
        private readonly IUploadErrorRepository _uploadErrorRepository;

        public UploadDataService(
            IAdvertisingPlatformParser fileParser,
            ILogger<UploadDataService> logger,
            IAdvertisementRepository advertisementRepository,
            ILocationRepository locationRepository,
            IAdvertisingPlatformRepository advertisingPlatformRepository,
            IUploadErrorRepository uploadErrorRepository)
        {
            _fileParser = fileParser;
            _logger = logger;
            _advertisementRepository = advertisementRepository;
            _locationRepository = locationRepository;
            _advertisingPlatformRepository = advertisingPlatformRepository;
            _uploadErrorRepository = uploadErrorRepository;
        }

        public async Task UploadDataFromFile(IFileData fileData, string uploadSource, CancellationToken cancellationToken)
        {
            var (validDataItems, validationErrors) = await _fileParser.ParseFile(fileData, cancellationToken);

            var errors = await SaveDataToDb(validDataItems, cancellationToken);

            var allErrors = (validationErrors ?? Enumerable.Empty<UploadErrorDto>())
                .Concat(errors)
                .ToList();

            await SaveUploadErrorsToDb(allErrors, uploadSource, cancellationToken);

            _logger.LogInformation(LogMessages.DATA_UPLOADED_SUCCESSFULLY);
        }

        public async Task UploadDataFromStream(Stream stream, string contentTypeOrExtension, string uploadSource, CancellationToken cancellationToken)
        {
            var (validDataItems, validationErrors) = await _fileParser.ParseStream(stream, contentTypeOrExtension, cancellationToken);

            var errors = await SaveDataToDb(validDataItems, cancellationToken);

            var allErrors = (validationErrors ?? Enumerable.Empty<UploadErrorDto>())
                .Concat(errors)
                .ToList();

            await SaveUploadErrorsToDb(allErrors, uploadSource, cancellationToken);

            _logger.LogInformation(LogMessages.DATA_UPLOADED_SUCCESSFULLY);
        }

        private async Task SaveUploadErrorsToDb(IReadOnlyList<UploadErrorDto>? errorDtos, string uploadSource, CancellationToken cancellationToken)
        {
            if (errorDtos == null || errorDtos.Count == 0)
            {
                return;
            }
            var errorEntyties = errorDtos.Select(e =>
                new UploadErrorDb(uploadSource, 
                e.RawData, 
                e.ErrorType, 
                e.ErrorMessage)).ToList();

            await _uploadErrorRepository.AddRange(errorEntyties, cancellationToken);
        }

        private async Task<List<UploadErrorDto>> SaveDataToDb(IReadOnlyList<ParseDataDto>? parseDataItems, 
            CancellationToken cancellationToken)
        {
            var errors = new List<UploadErrorDto>();

            if (parseDataItems == null || parseDataItems.Count == 0)
            {
                errors.Add(new UploadErrorDto(
                    rawData: "",
                    errorType: ExceptionTypes.VALIDATION_ERROR,
                    errorMessage: ErrorMessages.NO_DATA_TO_DOWNLOAD));

                _logger.LogInformation(ErrorMessages.NO_DATA_TO_DOWNLOAD);
                return errors;
            }
            var locationDictionary = await CreateLocations(parseDataItems, errors, cancellationToken);
            var advertisementDictionary = await CreateAdvertisements(parseDataItems, errors, cancellationToken);
            await CreateAdvettisingPlatform(parseDataItems, locationDictionary, advertisementDictionary, errors, cancellationToken);

            return errors;
        }

        private async Task<Dictionary<string, LocationDb>> CreateLocations(IReadOnlyCollection<ParseDataDto> parseData,
            List<UploadErrorDto> duplicateErrors,
            CancellationToken cancellationToken)
        {
            var locationDictionary = new Dictionary<string, LocationDb>();
            var newLocations = new List<LocationDb>();
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
                    duplicateErrors.Add(new UploadErrorDto( 
                        rawData: path,
                        errorType: ExceptionTypes.DUPLICATE_ERROR,
                        errorMessage: $"{ErrorMessages.ENTITY_ALREADY_EXISTS}: {path}"));
                    skippedCount++;
                    continue;
                }

                var parent = await FindParentLocation(path, locationDictionary, newLocations, duplicateErrors, cancellationToken);
                var location = new LocationDb(path, parent?.Id);

                locationDictionary.Add(path, location);
                newLocations.Add(location);
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

            if (newLocations.Count > 0)
            {
                await _locationRepository.AddRange(newLocations, cancellationToken);
            }

            _logger.LogInformation(LogMessages.LOCATIONS_UPLOAD_SUMMARY, addedCount, skippedCount);
            return locationDictionary;
        }

        private async Task<LocationDb?> FindParentLocation(string path, Dictionary<string, LocationDb> locations, 
            List<LocationDb> newLocations,
            List<UploadErrorDto> duplicateErrors,
            CancellationToken cancellationToken)
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
                duplicateErrors.Add(new UploadErrorDto(
                    rawData: parentPath,
                    errorType: ExceptionTypes.DUPLICATE_ERROR,
                    errorMessage: $"{ErrorMessages.ENTITY_ALREADY_EXISTS}: {parentPath}"));
                return existingParent;
            }

            var newParent = await FindParentLocation(parentPath, locations, newLocations, duplicateErrors, cancellationToken);
            
            if (newParent == null)
            {
                newParent = new LocationDb(parentPath, null);
                locations.Add(parentPath, newParent);
                newLocations.Add(newParent);
                _logger.LogInformation(LogMessages.LOCATION_CREATED, parentPath);
            }

            return newParent;
        }

        private async Task<Dictionary<string, AdvertisementDb>> CreateAdvertisements(IReadOnlyCollection<ParseDataDto> parseData,
            List<UploadErrorDto> duplicateErrors,
            CancellationToken cancellationToken)
        {
            var advertisementDictionary = new Dictionary<string, AdvertisementDb>();
            var newAdvertisements = new List<AdvertisementDb>();
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
                    duplicateErrors.Add(new UploadErrorDto(
                        rawData: name,
                        errorType: ExceptionTypes.DUPLICATE_ERROR,
                        errorMessage: $"{ErrorMessages.ENTITY_ALREADY_EXISTS}: {name}"));
                    skippedCount++;
                    continue;
                }

                var advertisement = new AdvertisementDb(name);
                advertisementDictionary.Add(advertisement.Name, advertisement);
                newAdvertisements.Add(advertisement);
                _logger.LogInformation(LogMessages.ADVERTISEMENT_CREATED, name);
                addedCount++;
            }

            if (newAdvertisements.Count > 0)
            {
                await _advertisementRepository.AddRange(newAdvertisements, cancellationToken);
            }

            _logger.LogInformation(LogMessages.ADVERTISEMENTS_UPLOAD_SUMMARY, addedCount, skippedCount);
            return advertisementDictionary;
        }

        private async Task CreateAdvettisingPlatform(IReadOnlyCollection<ParseDataDto> parseData, 
            Dictionary<string, LocationDb> locations,
            Dictionary<string, AdvertisementDb> advertisements,
            List<UploadErrorDto> duplicateErrors,
            CancellationToken cancellationToken)
        {
            var newPlatforms = new List<AdvertisingPlatformDb>();
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
                        duplicateErrors.Add(new UploadErrorDto(
                            rawData: $"{dto.AdvertisementName} | {normalizedPath}",
                            errorType: ExceptionTypes.DUPLICATE_ERROR,
                            errorMessage: $"{ErrorMessages.ENTITY_ALREADY_EXISTS}: {dto.AdvertisementName} | {normalizedPath}"));
                        skippedCount++;
                        continue;
                    }

                    var platform = new AdvertisingPlatformDb(advertisement.Id, location.Id)
                    {
                        Advertisement = advertisement,
                        Location = location
                    };

                    newPlatforms.Add(platform);
                    _logger.LogInformation(LogMessages.PLATFORM_CREATED, 
                        dto.AdvertisementName, normalizedPath);
                    addedCount++;
                }
            }
            if (newPlatforms.Count > 0)
            {
                await _advertisingPlatformRepository.AddRange(newPlatforms, cancellationToken);
            }

            _logger.LogInformation(LogMessages.PLATFORMS_UPLOAD_SUMMARY, addedCount, skippedCount);
        }
    }
}
