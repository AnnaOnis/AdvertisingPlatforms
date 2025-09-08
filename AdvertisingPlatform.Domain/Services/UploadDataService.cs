using AdvertisingPlatforms.Base.Constants;
using AdvertisingPlatforms.Base.Extensions;
using AdvertisingPlatforms.DAL.Abstractions;
using AdvertisingPlatforms.DAL.Entities;
using AdvertisingPlatforms.Domain.Abstractions;
using AdvertisingPlatforms.Domain.DTOs;
using AdvertisingPlatforms.Domain.Enums;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

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
            var parsingResult = await _fileParser.ParseFile(fileData, cancellationToken);

            var errors = await SaveDataToDb(parsingResult.ValidData, cancellationToken);

            var allErrors = (parsingResult.Errors ?? Enumerable.Empty<ErrorDataDto>())
                .Concat(errors)
                .ToList();

            await SaveUploadErrorsToDb(allErrors, uploadSource, cancellationToken);

            _logger.LogInformation(LogMessages.DATA_UPLOADED_SUCCESSFULLY);
        }

        public async Task UploadDataFromStream(Stream stream, string contentTypeOrExtension, string uploadSource, CancellationToken cancellationToken)
        {
            var parsingResult = await _fileParser.ParseStream(stream, contentTypeOrExtension, cancellationToken);

            var stopwatch = Stopwatch.StartNew();
            try
            {
                var errors = await SaveDataToDb(parsingResult.ValidData, cancellationToken);

                var allErrors = (parsingResult.Errors ?? Enumerable.Empty<ErrorDataDto>())
                    .Concat(errors)
                    .ToList();

                await SaveUploadErrorsToDb(allErrors, uploadSource, cancellationToken);

                _logger.LogInformation(LogMessages.DATA_UPLOADED_SUCCESSFULLY);
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation($"Сохранение данных в бд выполнено за {stopwatch.ElapsedMilliseconds} мс");
            }

        }

        private async Task SaveUploadErrorsToDb(IReadOnlyList<ErrorDataDto>? errorDtos, string uploadSource, CancellationToken cancellationToken)
        {
            if (errorDtos == null || errorDtos.Count == 0)
            {
                return;
            }
            var errorEntyties = errorDtos.Select(e =>
                new UploadErrorDb(uploadSource, 
                e.RawData, 
                e.Type.ToString(), 
                e.ErrorMessage)).ToList();

            await _uploadErrorRepository.BulkInsert(errorEntyties, cancellationToken);
        }

        private async Task<List<ErrorDataDto>> SaveDataToDb(IReadOnlyList<ValidDataDto>? parseDataItems, 
            CancellationToken cancellationToken)
        {
            var errors = new List<ErrorDataDto>();

            if (parseDataItems == null || parseDataItems.Count == 0)
            {
                errors.Add(new ErrorDataDto(
                    rawData: "",
                    errorType: ErrorType.EmptyData,
                    errorMessage: ErrorMessages.EMPTY_DATA));

                _logger.LogInformation(ErrorMessages.EMPTY_DATA);
                return errors;
            }
            var locationDictionary = await CreateLocations(parseDataItems, errors, cancellationToken);
            var advertisementDictionary = await CreateAdvertisements(parseDataItems, errors, cancellationToken);
            await CreateAdvettisingPlatforms(parseDataItems, locationDictionary, advertisementDictionary, errors, cancellationToken);

            return errors;
        }

        private async Task<Dictionary<string, LocationDb>> CreateLocations(IReadOnlyCollection<ValidDataDto> parseData,
            List<ErrorDataDto> duplicateErrors,
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

            var existingLocations = await _locationRepository.GetAll(cancellationToken);
            var existingPaths = existingLocations.ToDictionary(location => location.Path);

            foreach (var path in sortedPaths)
            {
                if (locationDictionary.ContainsKey(path)) continue;

                if (existingPaths.ContainsKey(path))
                {
                    var existingLocation = existingPaths[path];
                    locationDictionary.Add(path, existingLocation);
                    //_logger.LogInformation(LogMessages.LOCATION_ALREADY_EXISTS, path);
                    duplicateErrors.Add(new ErrorDataDto( 
                        rawData: path,
                        errorType: ErrorType.DuplicateError,
                        errorMessage: $"{ErrorMessages.ENTITY_ALREADY_EXISTS}: {path}"));
                    skippedCount++;
                    continue;
                }

                var parent = await FindParentLocation(path, locationDictionary, newLocations, existingPaths, duplicateErrors, cancellationToken);
                var location = new LocationDb(path, parent?.Id);

                locationDictionary.Add(path, location);
                newLocations.Add(location);
                //_logger.LogInformation(LogMessages.LOCATION_CREATED, path);
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
                await _locationRepository.BulkInsert(newLocations, cancellationToken);
            }

            _logger.LogInformation(LogMessages.LOCATIONS_UPLOAD_SUMMARY, addedCount, skippedCount);
            return locationDictionary;
        }

        private async Task<LocationDb?> FindParentLocation(string path, 
            Dictionary<string, LocationDb> locations, 
            List<LocationDb> newLocations,
            Dictionary<string, LocationDb> existingPaths,
            List<ErrorDataDto> duplicateErrors,
            CancellationToken cancellationToken)
        {
            var segments = path.Split(TextSeparators.SLASH, StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length <= 1) return null;

            var parentPath = string.Join(TextSeparators.SLASH, segments.Take(segments.Length - 1));
            parentPath = parentPath.NormalizeLocationPath();

            if (locations.TryGetValue(parentPath, out var parent))
                return parent;

            if (existingPaths.ContainsKey(parentPath))
            {
                var existingParent = existingPaths[parentPath];
                locations.Add(parentPath, existingParent);
                //_logger.LogInformation(LogMessages.LOCATION_ALREADY_EXISTS, parentPath);
                duplicateErrors.Add(new ErrorDataDto(
                    rawData: parentPath,
                    errorType: ErrorType.DuplicateError,
                    errorMessage: $"{ErrorMessages.ENTITY_ALREADY_EXISTS}: {parentPath}"));
                return existingParent;
            }

            var newParent = await FindParentLocation(parentPath, locations, newLocations, existingPaths, duplicateErrors, cancellationToken);
            
            if (newParent == null)
            {
                newParent = new LocationDb(parentPath, null);
                locations.Add(parentPath, newParent);
                newLocations.Add(newParent);
                //_logger.LogInformation(LogMessages.LOCATION_CREATED, parentPath);
            }

            return newParent;
        }

        private async Task<Dictionary<string, AdvertisementDb>> CreateAdvertisements(IReadOnlyCollection<ValidDataDto> parseData,
            List<ErrorDataDto> duplicateErrors,
            CancellationToken cancellationToken)
        {
            var advertisementDictionary = new Dictionary<string, AdvertisementDb>();
            var newAdvertisements = new List<AdvertisementDb>();
            var uniqueNames = parseData.Select(d => d.AdvertisementName).Distinct();
            var addedCount = 0;
            var skippedCount = 0;

            var existingAdvertisements = await _advertisementRepository.GetAll(cancellationToken);
            var existingNames = existingAdvertisements.ToDictionary(advertisement => advertisement.Name);

            foreach (var name in uniqueNames)
            {
                if (existingNames.ContainsKey(name))
                {
                    var existingAdvertisement = existingNames[name];
                    advertisementDictionary.Add(name, existingAdvertisement);
                   // _logger.LogInformation(LogMessages.ADVERTISEMENT_ALREADY_EXISTS, name);
                    duplicateErrors.Add(new ErrorDataDto(
                        rawData: name,
                        errorType: ErrorType.DuplicateError,
                        errorMessage: $"{ErrorMessages.ENTITY_ALREADY_EXISTS}: {name}"));
                    skippedCount++;
                    continue;
                }

                var advertisement = new AdvertisementDb(name);
                advertisementDictionary.Add(advertisement.Name, advertisement);
                newAdvertisements.Add(advertisement);
                //_logger.LogInformation(LogMessages.ADVERTISEMENT_CREATED, name);
                addedCount++;
            }

            if (newAdvertisements.Count > 0)
            {
                await _advertisementRepository.BulkInsert(newAdvertisements, cancellationToken);
            }

            _logger.LogInformation(LogMessages.ADVERTISEMENTS_UPLOAD_SUMMARY, addedCount, skippedCount);
            return advertisementDictionary;
        }

        private async Task CreateAdvettisingPlatforms(IReadOnlyCollection<ValidDataDto> parseData, 
            Dictionary<string, LocationDb> locations,
            Dictionary<string, AdvertisementDb> advertisements,
            List<ErrorDataDto> duplicateErrors,
            CancellationToken cancellationToken)
        {
            var newPlatforms = new List<AdvertisingPlatformDb>();
            var skippedCount = 0;
            var addedCount = 0;

            var existingPlatforms = await _advertisingPlatformRepository.GetAll(cancellationToken);

            var existingPlatformKeys = existingPlatforms
                .Select(p => (p.AdvertisementId, p.LocationId))
                .ToHashSet();

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

                    var platformKey = (advertisement.Id, location.Id);

                    if (existingPlatformKeys.Contains(platformKey))
                    {
                        //_logger.LogInformation(LogMessages.PLATFORM_ALREADY_EXISTS, 
                            //dto.AdvertisementName, normalizedPath);
                        duplicateErrors.Add(new ErrorDataDto(
                            rawData: $"{dto.AdvertisementName} | {normalizedPath}",
                            errorType: ErrorType.DuplicateError,
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
                    //_logger.LogInformation(LogMessages.PLATFORM_CREATED, 
                     //   dto.AdvertisementName, normalizedPath);
                    addedCount++;
                }
            }
            if (newPlatforms.Count > 0)
            {
                await _advertisingPlatformRepository.BulkInsert(newPlatforms, cancellationToken);
            }

            _logger.LogInformation(LogMessages.PLATFORMS_UPLOAD_SUMMARY, addedCount, skippedCount);
        }
    }
}
