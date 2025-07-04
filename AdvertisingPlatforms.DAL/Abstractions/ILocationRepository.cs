using AdvertisingPlatforms.DAL.Entities;

namespace AdvertisingPlatforms.DAL.Abstractions
{
    public interface ILocationRepository : IRepository<LocationDb>
    {
        Task<LocationDb?> FindByPath(string path, CancellationToken cancellationToken);
        Task<bool> ExistsByPath(string path, CancellationToken cancellationToken);
    }
}
