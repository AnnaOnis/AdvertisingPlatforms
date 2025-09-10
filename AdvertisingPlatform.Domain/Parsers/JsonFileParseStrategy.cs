using AdvertisingPlatforms.Base.Constants;
using AdvertisingPlatforms.Base.Exceptions;
using AdvertisingPlatforms.Base.Extensions;
using AdvertisingPlatforms.Domain.Abstractions;
using AdvertisingPlatforms.Domain.DTOs;
using AdvertisingPlatforms.Domain.Enums;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;

namespace AdvertisingPlatforms.Domain.Parsers
{
    public class JsonFileParseStrategy : IFileParseStrategy
    {
        private readonly ILogger<JsonFileParseStrategy> _logger;
        private readonly JsonSerializerOptions _options;

        public JsonFileParseStrategy(ILogger<JsonFileParseStrategy> logger)
        {
            _logger = logger;
            _options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };
        }

        public bool CanParse(IFileData fileData) => fileData.ContentType == "application/json" ||
                   Path.GetExtension(fileData.FileName).Equals(".json", StringComparison.OrdinalIgnoreCase);

        public bool CanParse(string contentTypeOrExtension) => contentTypeOrExtension == "application/json" ||
                   contentTypeOrExtension.Equals(".json", StringComparison.OrdinalIgnoreCase);

        public async Task<ParsingResult> Parse(IFileData fileData, CancellationToken cancellationToken)
        {
            using var stream = await fileData.FileToMemoryStreamAsync(cancellationToken);
            return await Parse(stream, cancellationToken);
        }

        public async Task<ParsingResult> Parse(Stream stream, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();

            var result = new ParsingResult();

            try
            {
                var parsedData = await JsonSerializer.DeserializeAsync<List<ValidDataDto>>(stream, _options, cancellationToken);

                if (parsedData == null)
                {
                    result.Errors.Add(new ErrorDataDto(
                        string.Empty,
                        ErrorType.InvalidFormat,
                        ErrorMessages.INVALID_DATA_FORMAT));
                    return result;
                }

                if (parsedData.Count == 0)
                {
                    _logger.LogWarning(ErrorMessages.EMPTY_DATA);
                    result.Errors.Add(new ErrorDataDto(
                        string.Empty,
                        ErrorType.EmptyData,
                        ErrorMessages.EMPTY_DATA));
                    return result;
                }

                foreach (var item in parsedData)
                {
                    try
                    {
                        result.ValidData.Add(ValidateAndNormalizeParsedDataDto(item));
                    }
                    catch (DomainValidationException ex)
                    {
                        string raw = item != null ? JsonSerializer.Serialize(item) : "null";
                        result.Errors.Add(new ErrorDataDto(
                                raw,
                                ErrorType.ValidationError,
                                ex.Message));
                    }
                }               
            }
            finally
            {
                stopwatch.Stop();
                result.ElapsedTime = stopwatch.Elapsed;
                _logger.LogInformation($"Парсинг выполнен за {stopwatch.ElapsedMilliseconds} мс");               
            }
           return result;
        }

        private ValidDataDto ValidateAndNormalizeParsedDataDto(ValidDataDto item)
        {
            if (item == null)
                throw new DomainValidationException(ErrorMessages.ENTITY_CAN_NOT_BE_NULL);

            item.AdvertisementName.NormalizeAdvertisementName();
            item.AdvertisementName.ValidatePlatformName();

            if (item.LocationPaths == null || !item.LocationPaths.Any())
                throw new DomainValidationException(ErrorMessages.EMPTY_LOCATIONS_COLLECTION_FOR_PLATFORM);

            item.LocationPaths = item.LocationPaths
                .Select(path =>
                {
                    path.NormalizeLocationPath();
                    path.ValidateLocation();
                    return path;
                })
                .ToList();

            return item;
        }
    }
}
