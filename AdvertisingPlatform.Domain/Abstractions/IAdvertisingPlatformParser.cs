using AdvertisingPlatforms.DAL.Entities;
using AdvertisingPlatforms.Domain.DTOs;

namespace AdvertisingPlatforms.Domain.Abstractions
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
        IReadOnlyList<ParseDataDTO> ParseFile(Stream stream);
    }
}
