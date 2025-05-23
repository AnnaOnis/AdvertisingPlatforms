using AdvertisingPlatforms.DAL.Entities;
using AdvertisingPlatforms.Domain.Exceptions;
using AdvertisingPlatforms.Domain.Extensions;
using AdvertisingPlatforms.Domain.Abstractions;
using Microsoft.Extensions.Logging;
using AdvertisingPlatforms.DAL.Extensions;

namespace AdvertisingPlatforms.Domain.Parser
{

    public class PlatformsFileParser : IAdvertisingPlatformParser
    {
        private const char _SeparatorColon = ':';
        private const char _SeparatorComma = ',';

        private readonly ILogger<PlatformsFileParser> _logger;

        public PlatformsFileParser(ILogger<PlatformsFileParser> logger)
        {
            _logger = logger;
        }

        public IReadOnlyList<AdvertisingPlatform> ParseFile(Stream stream)
        {
            using var reader = new StreamReader(stream);

            var content = reader.ReadToEnd();
            var lines = content.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries);

            return lines
                .Select((line, index) => ParseLine(line))
                .Where(platform => platform != null)
                .ToList()
                .AsReadOnly();
        }

        private AdvertisingPlatform ParseLine(string line)
        {
            line.ValidateContentLine();

            var parts = line.Split(_SeparatorColon);
            var name = parts[0].Trim();
            name.ValidatePlatformName();

            Advertisement advertisement = new Advertisement(name);

            var locations = parts[1].Split(_SeparatorComma)
                .Select(l => l.Trim())
                .Where(l => !string.IsNullOrEmpty(l))
                .Select(l =>
                {
                    l.ValidateLocation();
                    l = l.NormalizeLocationPath();
                    return new Location(l);
                }).ToList().AsReadOnly();

            if (locations.Count == 0)
                throw new DomainValidationException("At least one valid location required");

            return new AdvertisingPlatform(advertisement, locations);
        }
    }
}
