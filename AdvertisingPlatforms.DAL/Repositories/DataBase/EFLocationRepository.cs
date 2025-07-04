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

        public Task<LocationDb?> FindByPath(string path, CancellationToken cancellationToken)
        {
            return Entities.FirstOrDefaultAsync(location => location.Path == path, cancellationToken);
        }

        public async Task<bool> ExistsByPath(string path, CancellationToken cancellationToken)
        {
            return await Entities.AnyAsync(location => location.Path == path, cancellationToken);
        }
    }
}
