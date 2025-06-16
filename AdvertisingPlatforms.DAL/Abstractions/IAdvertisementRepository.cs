using AdvertisingPlatforms.DAL.Entities;

namespace AdvertisingPlatforms.DAL.Abstractions
{
    public interface IAdvertisementRepository : IRepository<AdvertisementDb>
    {
        Task<AdvertisementDb?> FindByNameAsync (string name, CancellationToken cancellationToken);
    }
}
