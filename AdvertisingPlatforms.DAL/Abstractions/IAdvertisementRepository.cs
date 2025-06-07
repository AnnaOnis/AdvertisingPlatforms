using AdvertisingPlatforms.DAL.Entities;

namespace AdvertisingPlatforms.DAL.Abstractions
{
    public interface IAdvertisementRepository : IRepository<Advertisement>
    {
        Task<Advertisement> FindByNameAsync (string name, CancellationToken cancellationToken);
    }
}
