
using AdvertisingPlatforms.DAL.Abstractions;
using AdvertisingPlatforms.DAL.Entities;
using AdvertisingPlatforms.Domain.Abstractions;
using AdvertisingPlatforms.Domain.Models;

namespace AdvertisingPlatforms.Domain.Fabrics
{
    public class AdvertisementFactory : IDomainModelFactory<AdvertisementDb, Advertisement>
    {
        public Advertisement Create(AdvertisementDb entityDb)
        {
            return new Advertisement(entityDb.Id, entityDb.Name);
        }

        public IReadOnlyCollection<Advertisement> CreateMany(IReadOnlyCollection<AdvertisementDb> entityDbs)
        {
            if (!entityDbs.Any()) return [];
            return entityDbs.Select(Create).ToList();
        }
    }
}
