using AdvertisingPlatforms.Base.Exceptions;
using AdvertisingPlatforms.Base.Constants;
using AdvertisingPlatforms.DAL.Abstractions;
using AdvertisingPlatforms.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace AdvertisingPlatforms.DAL.Repositories.DataBase
{
    public class EFAdvertisementRepository : EFRepository<AdvertisementDb>, IAdvertisementRepository
    {
        public EFAdvertisementRepository(AdvertisingPlatformsDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task AddAsync(AdvertisementDb advertisementDb, CancellationToken cancellationToken)
        {
            if (await ExistsAsync(advertisementDb.Id, cancellationToken))
            {
                throw new EntityAlreadyExistsExeption(ErrorMessages.ENTITY_ALREADY_EXISTS + advertisementDb.Id);
            }

            if (await ExistsByNameAsync(advertisementDb.Name, cancellationToken))
            {
                throw new EntityAlreadyExistsExeption($"Advertisement with name '{advertisementDb.Name}' already exists");
            }

            await Entities.AddAsync(advertisementDb, cancellationToken);
        }

        public async Task<AdvertisementDb?> FindByNameAsync(string name, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            var advertisement = await Entities.FirstOrDefaultAsync(a => a.Name == name, cancellationToken);

            return advertisement;
        }

        public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            return await Entities.AnyAsync(a => a.Name == name, cancellationToken);
        }
    }
}
