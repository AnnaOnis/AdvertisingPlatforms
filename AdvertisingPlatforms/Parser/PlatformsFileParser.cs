

using AdvertisingPlatforms.Domain.Entities;
using AdvertisingPlatforms.Domain.Extensions;

namespace AdvertisingPlatforms.Parser
{
    /// <summary>
    /// Парсер текстовых файлов с рекламными площадками
    /// </summary>
    public class PlatformsFileParser
    {
        private const char _separatorColon = ':';
        private const char _separatorComma = ',';

        private readonly ILogger<PlatformsFileParser> _logger;

        public PlatformsFileParser(ILogger<PlatformsFileParser> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Парсит поток данных с информацией о площадках
        /// </summary>
        /// <param name="stream">Поток данных с текстовой информацией</param>
        /// <returns>Список распарсенных площадок</returns>
        public IReadOnlyList<AdvertisingPlatform> ParseFile(Stream stream)
        {
            using var reader = new StreamReader(stream);

            var content = reader.ReadToEnd();
            var lines = content.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries);

            return lines
                .Select((line, index) => ParseLine(line, index + 1))
                .Where(platform => platform != null)
                .ToList()
                .AsReadOnly();
        }

        private AdvertisingPlatform ParseLine(string line, int lineNumber)
        {
            line.ValidateContentLine(lineNumber);

            var parts = line.Split(_separatorColon);
            var name = parts[0].Trim();
            name.ValidatePlatformName(lineNumber);

            var locations = parts[1].Split(_separatorComma)
                .Select(l => l.Trim())
                .Where(l => !string.IsNullOrEmpty(l))
                .Select(l =>
                {
                    l.ValidateLocation(lineNumber);
                    return new Location(l);
                }).ToList().AsReadOnly();

            return new AdvertisingPlatform(name, locations);
        }
    }
}
