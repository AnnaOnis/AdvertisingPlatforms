
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

        public async Task<AdvertisementDb?> FindByNameAsync(string name, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            var advertisement = await Entities.FirstOrDefaultAsync(a => a.Name == name, cancellationToken);

            return advertisement;
        }
    }
}
