
namespace AdvertisingPlatforms.Domain.Abstractions
{
    public interface IFileData
    {
        string FileName { get; }
        long FileLength { get; }
        string ContentType { get; }
        Task<MemoryStream> FileToMemoryStreamAsync(CancellationToken cancellationToken);
    }
}
