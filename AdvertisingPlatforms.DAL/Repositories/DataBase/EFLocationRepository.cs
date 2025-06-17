using AdvertisingPlatforms.Base.Exceptions;
using AdvertisingPlatforms.Base.Constants;
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

        public override async Task AddAsync(LocationDb locationDb, CancellationToken cancellationToken)
        {
            if (await ExistsAsync(locationDb.Id, cancellationToken))
            {
                throw new EntityAlreadyExistsExeption(ErrorMessages.ENTITY_ALREADY_EXISTS + locationDb.Id);
            }

            if (await ExistsByPathAsync(locationDb.Path, cancellationToken))
            {
                throw new EntityAlreadyExistsExeption($"Location with path '{locationDb.Path}' already exists");
            }

            await Entities.AddAsync(locationDb, cancellationToken);
        }

        public Task<LocationDb?> FindByPathAsync(string path, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));

            return Entities.FirstOrDefaultAsync(location => location.Path == path, cancellationToken);
        }

        public async Task<bool> ExistsByPathAsync(string path, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));

            return await Entities.AnyAsync(location => location.Path == path, cancellationToken);
        }
    }
}
