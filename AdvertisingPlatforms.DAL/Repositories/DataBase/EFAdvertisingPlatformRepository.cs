
using AdvertisingPlatforms.Base.Extensions;
using AdvertisingPlatforms.DAL.Abstractions;
using AdvertisingPlatforms.DAL.Delegates;
using AdvertisingPlatforms.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace AdvertisingPlatforms.DAL.Repositories.DataBase
{
    public class EFAdvertisingPlatformRepository : EFRepository<AdvertisingPlatformDb>, IAdvertisingPlatformRepository
    {
        public EFAdvertisingPlatformRepository(AdvertisingPlatformsDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IReadOnlyCollection<AdvertisingPlatformDb>> FindByLocationAsync(LocationDb location, 
            CancellationToken cancellationToken, 
            AdvertisingPlatformsSortDelegate? sortDelegate = null)
        {
            IQueryable<AdvertisingPlatformDb> query = Entities;

            if (sortDelegate != null)
            {
                query = sortDelegate(query);
            }

            var finalQuery = query
                    .Include(platform => platform.Advertisement)
                    .Include(platform => platform.Location)
                    .Where(p => p.Location.Path.StartsWith(location.Path));
            
            finalQuery.ToQueryString();
            var platforms = await finalQuery.ToListAsync(cancellationToken);

            return platforms;
        }

        public override async Task<AdvertisingPlatformDb> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await Entities
                .Include(platform => platform.Advertisement)
                .Include(platform => platform.Location)
                .FirstAsync(platform => platform.Id == id, cancellationToken);
        }

        public override async Task<IReadOnlyCollection<AdvertisingPlatformDb>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await  Entities
                .Include(platform => platform.Advertisement)
                .Include(platform => platform.Location)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsAsync(Guid advertisementId, Guid locationId, CancellationToken cancellationToken)
        {
            var result = await Entities.AnyAsync(
                platform => platform.LocationId == locationId 
                    && platform.AdvertisementId == advertisementId, 
                cancellationToken);

            return result;
        }
    }
}
