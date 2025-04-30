using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdvertisingPlatforms.Domain.Entities;
using AdvertisingPlatforms.Domain.Repositories;

namespace AdvertisingPlatforms.DAL
{
    public class AdvertisingPlatformRepositoryInMemory : IAdvertisingPlatformRepository
    {
        private readonly InMemoryAdvertisingPlatformStorage _storage;

        public AdvertisingPlatformRepositoryInMemory(InMemoryAdvertisingPlatformStorage storage)
        {
            _storage = storage;
        }

        public Task<IReadOnlyCollection<AdvertisingPlatform>> FindByLocation(Location location, CancellationToken cancellationToken)
        {
            return _storage.FindPlatformsByLocation(location, cancellationToken);
        }

        public Task Save(IReadOnlyList<AdvertisingPlatform> platforms, CancellationToken cancellationToken)
        {
            return _storage.StoreAdvertisingPlatforms(platforms, cancellationToken);
        }
    }
}
