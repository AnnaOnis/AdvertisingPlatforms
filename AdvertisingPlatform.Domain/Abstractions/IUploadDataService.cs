
namespace AdvertisingPlatforms.Domain.Abstractions
{
    public interface IUploadDataService
    {
        Task UploadDataFromFile(IFileData fileData, CancellationToken cancellationToken);
    }
}
