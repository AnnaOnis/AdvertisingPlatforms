
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
            if (entityDbs.Count == 0) return Array.Empty<Advertisement>();
            var result = new List<Advertisement>();
            foreach (var item in entityDbs)
            {
                result.Add(Create(item));
            }
            return result;
        }
    }
}
