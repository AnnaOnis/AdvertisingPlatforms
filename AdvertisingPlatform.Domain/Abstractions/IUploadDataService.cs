
namespace AdvertisingPlatforms.Domain.Abstractions
{
    public interface IUploadDataService
    {
        Task UploadDataFromFile(IFileData fileData, string uploadSource, CancellationToken cancellationToken);
        Task UploadDataFromStream(Stream stream, string contentTypeOrExtension, string uploadSource, CancellationToken cancellationToken);
    }
}
