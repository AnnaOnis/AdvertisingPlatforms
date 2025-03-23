using AdvertisingPlatforms.Models;

namespace AdvertisingPlatforms.Parser
{
    /// <summary>
    /// Парсер текстовых файлов с рекламными площадками
    /// </summary>
    public static class PlatformsFileParser
    {
        private static readonly ILogger _logger = LoggerFactory.Create(b => b.AddConsole())
        .CreateLogger("PlatformsFileParser");

        /// <summary>
        /// Парсит поток данных с информацией о площадках
        /// </summary>
        /// <param name="stream">Поток данных с текстовой информацией</param>
        /// <returns>Список распарсенных площадок</returns>
        public static List<AdvPlatform> ParseFile(Stream stream)
        {
            var platformDict = new Dictionary<string, AdvPlatform>();
            var reader = new StreamReader(stream);

            var lineNumber = 0;
            var line = string.Empty;

            while((line = reader.ReadLine()) is not null)
            {
                lineNumber++;

                if (string.IsNullOrWhiteSpace(line))
                {
                    _logger.LogDebug("Skipped empty line {LineNumber}", lineNumber);
                    continue;
                }

                var parts = line.Split(":", 2);
                if (parts.Length != 2)
                {
                    _logger.LogWarning("Invalid format in line {LineNumber}: '{Line}'", lineNumber, line);
                    continue;
                }

                var name = parts[0].Trim();
                if (string.IsNullOrWhiteSpace(name))
                {
                    _logger.LogWarning("Empty name in line {LineNumber}", lineNumber);
                    continue;
                }

                var locations = parts[1].Split(",");
                locations = locations.Select(l =>  l.Trim()).Where(l => !string.IsNullOrEmpty(l)).ToArray();
                if (locations.Length == 0)
                {
                    _logger.LogWarning("No valid locations in line {LineNumber}", lineNumber);
                    continue;
                }

                try
                {
                    if (platformDict.TryGetValue(name, out var existingPlatform))
                    {
                        var mergedLocations = existingPlatform.Locations
                            .Concat(locations)
                            .Distinct()
                            .ToArray();

                        platformDict[name] = new AdvPlatform(name, mergedLocations);
                        _logger.LogDebug("Merged locations for platform {PlatformName}", name);
                    }
                    else
                    {
                        platformDict[name] = new AdvPlatform(name, locations);
                        _logger.LogDebug("Added new platform {PlatformName}", name);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing line {LineNumber}: '{Line}'", lineNumber, line);
                }
            }

            _logger.LogInformation("Parsed {Count} platforms", platformDict.Count);
            return platformDict.Values.ToList();
        }
    }
}
