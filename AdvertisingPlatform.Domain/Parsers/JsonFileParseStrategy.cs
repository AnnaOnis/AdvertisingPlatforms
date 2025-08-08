using AdvertisingPlatforms.Base.Constants;
using AdvertisingPlatforms.Base.Exceptions;
using AdvertisingPlatforms.Base.Extensions;
using AdvertisingPlatforms.Domain.Abstractions;
using AdvertisingPlatforms.Domain.DTOs;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Xml.Linq;

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

        public bool CanParse(IFileData fileData)
        {
            return fileData.ContentType == "application/json" ||
                   Path.GetExtension(fileData.FileName).Equals(".json", StringComparison.OrdinalIgnoreCase);
        }

        public bool CanParse(string contentTypeOrExtension)
        {
            return contentTypeOrExtension == "application/json" ||
                   contentTypeOrExtension.Equals(".json", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<(List<ParseDataDto> Valid, List<UploadErrorDto> Errors)> Parse(IFileData fileData, CancellationToken cancellationToken)
        {
            using var stream = await fileData.FileToMemoryStreamAsync(cancellationToken);
            return await Parse(stream, cancellationToken);
        }

        public async Task<(List<ParseDataDto> Valid, List<UploadErrorDto> Errors)> Parse(Stream stream, CancellationToken cancellationToken)
        {
            var valid = new List<ParseDataDto>();
            var errors = new List<UploadErrorDto>();

            var parsedData = await JsonSerializer.DeserializeAsync<List<ParseDataDto>>(stream, _options, cancellationToken);

            if (parsedData == null)
            {
                errors.Add(new UploadErrorDto(
                    "",
                    ExceptionTypes.VALIDATION_ERROR,
                    "JSON-файл пуст или невалиден."));
                return (valid, errors);
            }

            if (parsedData.Count == 0)
            {
                _logger.LogWarning("JSON-файл корректен, но не содержит данных.");
                return (valid, errors);
            }

            foreach (var item in parsedData)
            {
                try
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

                    valid.Add(item);
                }
                catch (DomainValidationException ex)
                {
                    string raw = item != null ? JsonSerializer.Serialize(item) : "null";
                    errors.Add(new UploadErrorDto(
                            raw,
                            ExceptionTypes.VALIDATION_ERROR,
                            ex.Message));
                }

            }

            return (valid, errors);
        }
    }
}
