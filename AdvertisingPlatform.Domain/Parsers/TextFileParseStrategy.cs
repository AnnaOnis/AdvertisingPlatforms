using AdvertisingPlatforms.Base.Constants;
using AdvertisingPlatforms.Base.Exceptions;
using AdvertisingPlatforms.Base.Extensions;
using AdvertisingPlatforms.Domain.Abstractions;
using AdvertisingPlatforms.Domain.DTOs;
using AdvertisingPlatforms.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace AdvertisingPlatforms.Domain.Parsers
{
    public class TextFileParseStrategy : IFileParseStrategy
    {
        private readonly ILogger<TextFileParseStrategy> _logger;

        public TextFileParseStrategy(ILogger<TextFileParseStrategy> logger)
        {
            _logger = logger;
        }

        public bool CanParse(IFileData fileData) => fileData.ContentType == "text/plain" ||
                   Path.GetExtension(fileData.FileName).Equals(".txt", StringComparison.OrdinalIgnoreCase);

        public bool CanParse(string contentTypeOrExtension) => contentTypeOrExtension == "text/plain" ||
                   contentTypeOrExtension.Equals(".txt", StringComparison.OrdinalIgnoreCase);


        public async Task<ParsingResult> Parse(IFileData fileData, CancellationToken cancellationToken)
        {
            using var stream = await fileData.FileToMemoryStreamAsync(cancellationToken);
            return await Parse(stream, cancellationToken);
        }

        public async Task<ParsingResult> Parse(Stream stream, CancellationToken cancellationToken)
        {
            var result = new ParsingResult();

            using var reader = new StreamReader(stream);
            var content = await reader.ReadToEndAsync(cancellationToken);

            var lines = content.Split([TextSeparators.CONTENT_LINE_SEPARATOR_CRLF, 
                TextSeparators.CONTENT_LINE_SEPARATOR_LF ],
                StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                try
                {
                    var dto = ParseLine(line);
                    result.ValidData.Add(dto);
                }
                catch (DomainValidationException ex)
                {
                    _logger.LogError(ex, ErrorMessages.ERROR_PARSING_LINE, i + 1, line);
                    result.Errors.Add(new ErrorDataDto(
                        line,
                        ErrorType.ValidationError,
                        $"Line {i + 1}: {ex.Message}"));
                }
            }

            return result;
        }

        private static ValidDataDto ParseLine(string line)
        {
            line.ValidateContentLine();

            var parts = line.Split(TextSeparators.SEPARATOR_COLON, 2);
            if (parts.Length < 2)
                throw new DomainValidationException(ErrorMessages.INVALID_DATA_FORMAT);

            var name = ValidateAndNormalizeName(parts[0]);

            var locationPaths = ValidateAndNormalizeLocationPaths(parts[1]);

            return new ValidDataDto(name, locationPaths);
        }

        private static string ValidateAndNormalizeName(string name)
        {
            name = name.Trim();
            name.NormalizeAdvertisementName();
            name.ValidatePlatformName();
            return name;
        }

        private static List<string> ValidateAndNormalizeLocationPaths(string locationPaths)
        {
            var validLocationPaths = locationPaths.Split(TextSeparators.SEPARATOR_COMMA)
                .Select(locationPath =>
                {
                    locationPath = locationPath.Trim();
                    locationPath = locationPath.NormalizeLocationPath();
                    locationPath.ValidateLocation();
                    return locationPath;
                })
                .ToList();

            if (validLocationPaths.Count == 0)
                throw new DomainValidationException(ErrorMessages.EMPTY_LOCATIONS_COLLECTION_FOR_PLATFORM);

            return validLocationPaths;
        }
    }
}
