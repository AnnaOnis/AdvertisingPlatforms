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

        public override Task AddAsync(AdvertisementDb advertisementDb, CancellationToken cancellationToken)
        {
            if (_entityById.ContainsKey(advertisementDb.Id))
            {
                throw new EntityAlreadyExistsExeption(ErrorMessages.ENTITY_ALREADY_EXISTS + advertisementDb.Id);
            }

            if (ExistsByName(advertisementDb.Name))
            {
                throw new EntityAlreadyExistsExeption($"Advertisement with name '{advertisementDb.Name}' already exists");
            }

            return base.AddAsync(advertisementDb, cancellationToken);
        }

        public Task<AdvertisementDb?> FindByNameAsync(string name, CancellationToken cancellationToken)
        {
            if(string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            var item = _entityById.FirstOrDefault(item => item.Value.Name == name);

            return Task.FromResult(item.Value ?? null);
        }

        public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken)
        {
            if(string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            return Task.FromResult(ExistsByName(name));
        }

        private bool ExistsByName(string name)
        {
            return _entityById.Values.Any(item => item.Name == name);
        }
    }
}
