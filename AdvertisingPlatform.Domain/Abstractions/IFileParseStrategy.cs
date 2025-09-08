using AdvertisingPlatforms.Domain.DTOs;

namespace AdvertisingPlatforms.Domain.Abstractions
{
    public interface IFileParseStrategy
    {
        bool CanParse(IFileData fileData);
        bool CanParse(string contentTypeOrExtension);
        Task<ParsingResult> Parse(IFileData fileData, CancellationToken cancellationToken);
        Task<ParsingResult> Parse(Stream stream, CancellationToken cancellationToken);
    }
}
