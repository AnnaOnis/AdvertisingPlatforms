
using System;
using System.Collections.Concurrent;
using AdvertisingPlatforms.Base.Constants;
using AdvertisingPlatforms.Base.Exceptions;
using AdvertisingPlatforms.DAL.Abstractions;
using AdvertisingPlatforms.DAL.Entities;

namespace AdvertisingPlatforms.DAL.Repositories.InMemory
{
    public class InMemoryAdvertisementRepository : InMemoryRepository<AdvertisementDb>, IAdvertisementRepository
    {
        public InMemoryAdvertisementRepository() { }

        public Task<AdvertisementDb?> FindByNameAsync(string name, CancellationToken cancellationToken)
        {
            if(string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            var item = _entityById.FirstOrDefault(item => item.Value.Name == name);

            return Task.FromResult(item.Value ?? null);
        }
    }
}
