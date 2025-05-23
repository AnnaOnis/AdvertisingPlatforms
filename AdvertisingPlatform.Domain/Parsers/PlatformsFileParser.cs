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
            using var reader = new StreamReader(stream);

            var content = reader.ReadToEnd();
            var lines = content.Split([TextSeparators.CONTENT_LINE_SEPARATOR_CRLF, 
                TextSeparators.CONTENT_LINE_SEPARATOR_LF], 
                StringSplitOptions.RemoveEmptyEntries);

            return lines
                .Select((line, index) => ParseLine(line))
                .Where(platform => platform != null)
                .ToList()
                .AsReadOnly();
        }

        private AdvertisingPlatform ParseLine(string line)
        {
            line.ValidateContentLine();

            var parts = line.Split(TextSeparators.SEPARATOR_COLON);
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
                throw new DomainValidationException("At least one valid location required");

            return new AdvertisingPlatform(advertisement, locations);
        }
    }
}
