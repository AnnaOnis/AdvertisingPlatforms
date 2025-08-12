using AdvertisingPlatforms.DAL.Entities;

namespace AdvertisingPlatforms.DAL.Abstractions
{
    public interface IAdvertisementRepository : IRepository<AdvertisementDb>
    {
        Task<AdvertisementDb?> FindByName(string name, CancellationToken cancellationToken);
        Task<bool> ExistsByName(string name, CancellationToken cancellationToken);
    }
}
