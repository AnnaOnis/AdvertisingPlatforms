using AdvertisingPlatforms.DAL.Delegates;
using AdvertisingPlatforms.DAL.Entities;

namespace AdvertisingPlatforms.DAL.Abstractions
{
    /// <summary>
    /// Repository for working with advertising platforms
    /// </summary>
    public interface IAdvertisingPlatformRepository : IRepository<AdvertisingPlatformDb>
    {
        Task<IReadOnlyCollection<AdvertisingPlatformDb>> FindByLocationAsync(LocationDb location, 
            CancellationToken cancellationToken, 
            AdvertisingPlatformsSortDelegate? sortDelegate = null);

        Task<bool> ExistsByAdvertisementAndLocationAsync(Guid advertisementId, Guid locationId, CancellationToken cancellationToken);
    }
}
