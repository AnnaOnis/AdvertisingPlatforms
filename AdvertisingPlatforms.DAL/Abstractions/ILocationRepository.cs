using AdvertisingPlatforms.DAL.Entities;

namespace AdvertisingPlatforms.DAL.Abstractions
{
    public interface ILocationRepository : IRepository<Location>
    {
        Task<Location> FindByPathAsync(string path, CancellationToken cancellationToken);
    }
}
