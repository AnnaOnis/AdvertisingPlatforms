using AdvertisingPlatforms.DAL.Entities;

namespace AdvertisingPlatforms.Domain.Abstractions
{
    /// <summary>
    /// Service for working with advertising platforms
    /// </summary>
    public interface IAdvertisingPlatformService
    {
        /// <summary>
        /// Uploads new platform data
        /// </summary>
        /// <param name="platforms">Collection of advertising platforms</param>
        /// <param name="cancellationToken">Cancellation token</param>
        Task Upload(IReadOnlyList<AdvertisingPlatform> platforms, CancellationToken cancellationToken);

        /// <summary>
        /// Uploads advertising platform data from a text file.
        /// </summary>
        /// <param name="fileData">The input text file containing advertising platform data to upload.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>The number of advertising platforms successfully uploaded.</returns>
        Task<int> UploadFromFile(IFileData fileData, CancellationToken cancellationToken);

        /// <summary>
        /// Searches for platforms by specified location
        /// </summary>
        /// <param name="location">Target location</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Collection of matching platforms</returns>
        Task<IReadOnlyCollection<AdvertisingPlatform>> Search(Location location, CancellationToken cancellationToken);
    }
}
