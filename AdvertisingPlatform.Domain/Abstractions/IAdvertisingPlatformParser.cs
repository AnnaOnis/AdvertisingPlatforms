using AdvertisingPlatforms.Domain.Entities;

namespace AdvertisingPlatforms.Domain.Interfaces
{
    /// <summary>
    /// Parser for text files containing advertising platforms
    /// </summary>
    public interface IAdvertisingPlatformParser
    {
        /// <summary>
        /// Parses a data stream containing platform information
        /// </summary>
        /// <param name="stream">Data stream with text information</param>
        /// <returns>List of parsed platforms</returns>
        IReadOnlyList<AdvertisingPlatform> ParseFile(Stream stream);
    }
}
