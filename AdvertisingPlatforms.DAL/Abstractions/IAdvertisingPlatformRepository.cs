using AdvertisingPlatforms.DAL.Delegates;
using AdvertisingPlatforms.DAL.Entities;

namespace AdvertisingPlatforms.DAL.Abstractions
{
    /// <summary>
    /// Repository for working with advertising platforms
    /// </summary>
    public interface IAdvertisingPlatformRepository : IRepository<AdvertisingPlatformDb>
    {
        Task<IReadOnlyCollection<AdvertisingPlatformDb>> FindByLocation(LocationDb location, 
            CancellationToken cancellationToken, 
            AdvertisingPlatformsSortDelegate? sortDelegate = null);

        Task<bool> ExistsByAdvertisementAndLocation(Guid advertisementId, Guid locationId, CancellationToken cancellationToken);
    }
}
