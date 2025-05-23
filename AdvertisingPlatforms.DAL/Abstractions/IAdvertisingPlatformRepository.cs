using AdvertisingPlatforms.DAL.Entities;

namespace AdvertisingPlatforms.DAL.Abstractions
{
    /// <summary>
    /// Repository for working with advertising platforms
    /// </summary>
    public interface IAdvertisingPlatformRepository
    {
        /// <summary>
        /// Finds advertising platforms by specified location
        /// </summary>
        /// <param name="location">Geographic location to search</param>
        /// <param name="cancellationToken">Operation cancellation token</param>
        /// <returns>List of advertising platforms in the specified location</returns>
        Task<IReadOnlyCollection<AdvertisingPlatform>> FindByLocation(Location location, CancellationToken cancellationToken);

        /// <summary>
        /// Saves a list of advertising platforms
        /// </summary>
        /// <param name="platforms">List of platforms to save</param>
        /// <param name="cancellationToken">Operation cancellation token</param>
        Task Save(IReadOnlyList<AdvertisingPlatform> platforms, CancellationToken cancellationToken);
    }
}
