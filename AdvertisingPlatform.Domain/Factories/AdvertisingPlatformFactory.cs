using AdvertisingPlatforms.DAL.Entities;
using AdvertisingPlatforms.Domain.Abstractions;
using AdvertisingPlatforms.Domain.Models;

namespace AdvertisingPlatforms.Domain.Fabrics
{
    public class AdvertisingPlatformFactory : IDomainModelFactory<AdvertisingPlatformDb, AdvertisingPlatform>
    {
        public AdvertisingPlatform Create(AdvertisingPlatformDb entityDb)
        {
            return new AdvertisingPlatform(entityDb.Id, 
                entityDb.AdvertisementId, 
                entityDb.LocationId,
                entityDb.Advertisement?.Name ?? string.Empty,
                entityDb.Location?.Path ?? string.Empty);
        }

        public IReadOnlyCollection<AdvertisingPlatform> CreateMany(IReadOnlyCollection<AdvertisingPlatformDb> advertisingPlatformDbs)
        {
            if (advertisingPlatformDbs.Count == 0) return [];
            return advertisingPlatformDbs.Select(Create).ToList();
        }
    }
}
