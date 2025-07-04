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

        public Task<AdvertisementDb?> FindByName(string name, CancellationToken cancellationToken)
        {
            return Task.FromResult(_entityById.Values.FirstOrDefault(x => x.Name == name));
        }

        public Task<bool> ExistsByName(string name, CancellationToken cancellationToken)
        {
            return Task.FromResult(ExistsByName(name));
        }

        private bool ExistsByName(string name)
        {
            return _entityById.Values.Any(item => item.Name == name);
        }
    }
}
