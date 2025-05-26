using AdvertisingPlatforms.DAL.Entities;
using AdvertisingPlatforms.Base.Exceptions;
using AdvertisingPlatforms.Base.Extensions;
using AdvertisingPlatforms.Domain.Abstractions;
using Microsoft.Extensions.Logging;
using AdvertisingPlatforms.DAL.Extensions;
using AdvertisingPlatforms.Base.Constants;

namespace AdvertisingPlatforms.Domain.Parser
{

    public class PlatformsFileParser : IAdvertisingPlatformParser
    {
        private readonly ILogger<PlatformsFileParser> _logger;

        public PlatformsFileParser(ILogger<PlatformsFileParser> logger)
        {
            _logger = logger;
        }

        public IReadOnlyList<AdvertisingPlatform> ParseFile(Stream stream)
        {
            _logger.LogDebug(LogMessages.PARSING_FILE_CONTENT);
            using var reader = new StreamReader(stream);

            var content = reader.ReadToEnd();
            var lines = content.Split([TextSeparators.CONTENT_LINE_SEPARATOR_CRLF, 
                TextSeparators.CONTENT_LINE_SEPARATOR_LF], 
                StringSplitOptions.RemoveEmptyEntries);

            var result = new List<AdvertisingPlatform>();
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                try
                {
                    var platform = ParseLine(line);
                    result.Add(platform);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ErrorMessages.ERROR_PARSING_LINE, i + 1, line);
                }
            }
            return result;
        }

        private AdvertisingPlatform ParseLine(string line)
        {
            line.ValidateContentLine();

            var parts = line.Split(TextSeparators.SEPARATOR_COLON, 2);
            var name = parts[0].Trim();
            name.ValidatePlatformName();

            var advertisement = new Advertisement(name);

            var locations = parts[1].Split(TextSeparators.SEPARATOR_COMMA)
                .Select(locationPath =>
                {
                    locationPath.ValidateLocation();
                    locationPath = locationPath.NormalizeLocationPath();
                    return new Location(locationPath);
                }).ToList();

            if (locations.Count == 0)
                throw new DomainValidationException(ErrorMessages.EMPTY_LOCATIONS_COLLECTION_FOR_PLATFORM);

            return new AdvertisingPlatform(advertisement, locations);
        }
    }
}
