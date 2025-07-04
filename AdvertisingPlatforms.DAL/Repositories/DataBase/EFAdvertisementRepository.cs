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

        public async Task<AdvertisementDb?> FindByName(string name, CancellationToken cancellationToken)
        {
            var advertisement = await Entities.FirstOrDefaultAsync(a => a.Name == name, cancellationToken);

            return advertisement;
        }

        public async Task<bool> ExistsByName(string name, CancellationToken cancellationToken)
        {
            return await Entities.AnyAsync(a => a.Name == name, cancellationToken);
        }
    }
}
