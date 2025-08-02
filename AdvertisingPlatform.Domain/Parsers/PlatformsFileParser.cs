using AdvertisingPlatforms.DAL.Entities;
using AdvertisingPlatforms.Base.Exceptions;
using AdvertisingPlatforms.Base.Extensions;
using AdvertisingPlatforms.Domain.Abstractions;
using Microsoft.Extensions.Logging;
using AdvertisingPlatforms.Base.Constants;
using AdvertisingPlatforms.Domain.DTOs;
using System.Text.Json;

namespace AdvertisingPlatforms.Domain.Parser
{

    public class PlatformsFileParser : IAdvertisingPlatformParser
    {
        private readonly ILogger<PlatformsFileParser> _logger;

        public PlatformsFileParser(ILogger<PlatformsFileParser> logger)
        {
            _logger = logger;
        }

        public IReadOnlyList<ParseDataDto> ParseFile(Stream stream)
        {
            _logger.LogDebug(LogMessages.PARSING_FILE_CONTENT);
            StreamReader reader = new StreamReader(stream);
            var content = reader.ReadToEnd();

            if(TryParseJson(content, out var result))
            {
                _logger.LogDebug("File parsed as JSON successfully");
                return result;
            }

            _logger.LogDebug("Parsing as text file");
            return ParseTextContent(content);
        }

        private bool TryParseJson(string content, out IReadOnlyList<ParseDataDto>? result)
        {
            try
            {
                var parsedData = JsonSerializer.Deserialize<List<ParseDataDto>>(content, GetJsonSerializerOptions());

                if (parsedData == null) 
                {
                    result = null;
                    return false;
                }

                if (parsedData.Count == 0)
                {
                    _logger.LogWarning("JSON-файл корректен, но не содержит данных.");
                    result = parsedData;
                    return true;
                }

                if (parsedData.Any(item => item == null))
                {
                    throw new DomainValidationException(ErrorMessages.ENTITY_CAN_NOT_BE_NULL);
                }

                foreach (var item in parsedData)
                {
                    item.AdvertisementName.ValidatePlatformName();

                    if (item.LocationPaths == null || !item.LocationPaths.Any())
                    {
                        throw new DomainValidationException(
                            ErrorMessages.EMPTY_LOCATIONS_COLLECTION_FOR_PLATFORM);
                    }

                    item.LocationPaths = item.LocationPaths
                        .Select(path =>
                        {
                            path.ValidateLocation();
                            return path.NormalizeLocationPath();
                        })
                        .ToList();
                }

                result = parsedData;
                return true;
            }
            catch (JsonException)
            {
                result = null;
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "JSON parsing error");
                throw; 
            }
        }

        private static JsonSerializerOptions GetJsonSerializerOptions()
        {
            return new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };
        }

        private IReadOnlyList<ParseDataDto> ParseTextContent(string content)
        {
            var lines = content.Split([TextSeparators.CONTENT_LINE_SEPARATOR_CRLF,
                TextSeparators.CONTENT_LINE_SEPARATOR_LF],
                StringSplitOptions.RemoveEmptyEntries);

            var result = new List<ParseDataDto>();
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                try
                {
                    var dto = ParseLine(line);
                    result.Add(dto);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ErrorMessages.ERROR_PARSING_LINE, i + 1, line);
                    throw;
                }
            }
            return result;
        }

        private static ParseDataDto ParseLine(string line)
        {
            line.ValidateContentLine();

            var parts = line.Split(TextSeparators.SEPARATOR_COLON, 2);
            var name = parts[0].Trim();
            name.ValidatePlatformName();

            var locationPaths = parts[1].Split(TextSeparators.SEPARATOR_COMMA)
                .Select(locationPath =>
                {
                    locationPath.ValidateLocation();
                    locationPath = locationPath.NormalizeLocationPath();
                    return locationPath;
                });

            if (!locationPaths.Any())
                throw new DomainValidationException(ErrorMessages.EMPTY_LOCATIONS_COLLECTION_FOR_PLATFORM);

            return new ParseDataDto(name, locationPaths);
        }
    }
}
