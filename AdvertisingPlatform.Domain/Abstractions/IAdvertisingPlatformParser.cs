using AdvertisingPlatforms.Domain.DTOs;

namespace AdvertisingPlatforms.Domain.Abstractions
{
    /// <summary>
    /// Parser for text files containing advertising platforms
    /// </summary>
    public interface IAdvertisingPlatformParser
    {
        /// <summary>
        /// Parses a file data containing platform information
        /// </summary>
        /// <param name="fileData">File with text information</param>
        /// <returns>List of parsed platforms</returns>
        Task<(List<ParseDataDto> Valid, List<UploadErrorDto> Errors)> ParseFile(IFileData fileData, CancellationToken cancellationToken);

        /// <summary>
        /// Parses a stream data containing platform information
        /// </summary>
        /// <param name="stream">Stream with platform information</param>
        /// <returns>List of parsed platforms</returns>
        Task<(List<ParseDataDto> Valid, List<UploadErrorDto> Errors)> ParseStream(Stream stream, string contentTypeOrExtension, CancellationToken cancellationToken);
    }
}
