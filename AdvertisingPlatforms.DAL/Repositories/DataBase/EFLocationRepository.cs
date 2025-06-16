using AdvertisingPlatforms.DAL.Abstractions;
using AdvertisingPlatforms.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace AdvertisingPlatforms.DAL.Repositories.DataBase
{
    public class EFLocationRepository : EFRepository<LocationDb>, ILocationRepository
    {
        public EFLocationRepository(AdvertisingPlatformsDbContext dbContext) : base(dbContext)
        {
        }

        public Task<LocationDb?> FindByPathAsync(string path, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));

            return Entities.FirstOrDefaultAsync(location => location.Path == path, cancellationToken);
        }
    }
}
