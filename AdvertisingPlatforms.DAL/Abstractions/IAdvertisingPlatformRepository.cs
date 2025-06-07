using AdvertisingPlatforms.DAL.Entities;

namespace AdvertisingPlatforms.DAL.Abstractions
{
    /// <summary>
    /// Repository for working with advertising platforms
    /// </summary>
    public interface IAdvertisingPlatformRepository : IRepository<AdvertisingPlatform>
    {
        /// <summary>
        /// Finds advertising platforms by specified location
        /// </summary>
        /// <param name="location">Geographic location to search</param>
        /// <param name="cancellationToken">Operation cancellation token</param>
        /// <returns>List of advertising platforms in the specified location</returns>
        Task<IReadOnlyCollection<AdvertisingPlatform>> FindByLocationAsync(Location location, CancellationToken cancellationToken);


    }
}
