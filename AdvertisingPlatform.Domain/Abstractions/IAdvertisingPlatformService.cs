

using AdvertisingPlatforms.Domain.Entities;

namespace AdvertisingPlatforms.Domain.Interfaces
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
        /// Searches for platforms by specified location
        /// </summary>
        /// <param name="location">Target location</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Collection of matching platforms</returns>
        Task<IReadOnlyCollection<AdvertisingPlatform>> Search(Location location, CancellationToken cancellationToken);
    }
}
