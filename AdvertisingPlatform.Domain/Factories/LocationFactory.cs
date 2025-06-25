
using AdvertisingPlatforms.DAL.Abstractions;
using AdvertisingPlatforms.DAL.Entities;
using AdvertisingPlatforms.Domain.Abstractions;
using AdvertisingPlatforms.Domain.Models;

namespace AdvertisingPlatforms.Domain.Fabrics
{
    public class LocationFactory : IDomainModelFactory<LocationDb, Location>
    {
        public Location Create(LocationDb entityDb)
        {
            return new Location(entityDb.Id, entityDb.Path, entityDb.ParentId);
        }

        public IReadOnlyCollection<Location> CreateMany(IReadOnlyCollection<LocationDb> entityDbs)
        {
            if (entityDbs.Count == 0) return [];
            return entityDbs.Select(Create).ToList();
        }
    }
}
