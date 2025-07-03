using AdvertisingPlatforms.DAL.Entities;

namespace AdvertisingPlatforms.DAL.Abstractions
{
    public interface ILocationRepository : IRepository<LocationDb>
    {
        Task<LocationDb?> FindByPathAsync(string path, CancellationToken cancellationToken);
        Task<bool> ExistsByPathAsync(string path, CancellationToken cancellationToken);
    }
}
