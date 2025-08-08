using AdvertisingPlatforms.Domain.DTOs;

namespace AdvertisingPlatforms.Domain.Abstractions
{
    public interface IFileParseStrategy
    {
        bool CanParse(IFileData fileData);
        bool CanParse(string contentTypeOrExtension);
        Task<(List<ParseDataDto> Valid, List<UploadErrorDto> Errors)> Parse(IFileData fileData, CancellationToken cancellationToken);
        Task<(List<ParseDataDto> Valid, List<UploadErrorDto> Errors)> Parse(Stream stream, CancellationToken cancellationToken);
    }
}
