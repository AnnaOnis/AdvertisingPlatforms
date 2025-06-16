
using System;
using System.Collections.Concurrent;
using AdvertisingPlatforms.Base.Constants;
using AdvertisingPlatforms.Base.Exceptions;
using AdvertisingPlatforms.DAL.Abstractions;
using AdvertisingPlatforms.DAL.Entities;

namespace AdvertisingPlatforms.DAL.Repositories.InMemory
{
    public class InMemoryLocationRepository : InMemoryRepository<LocationDb>, ILocationRepository
    {
        public InMemoryLocationRepository() { }

        public Task<LocationDb?> FindByPathAsync(string path, CancellationToken cancellationToken)
        {
            if(string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));

            var item = _entityById.FirstOrDefault(item => item.Value.Path == path);

            return Task.FromResult(item.Value ?? null);
        }
    }
}
