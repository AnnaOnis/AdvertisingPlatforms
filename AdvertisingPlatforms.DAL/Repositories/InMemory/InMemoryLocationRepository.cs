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

        public override Task AddAsync(LocationDb locationDb, CancellationToken cancellationToken)
        {
            if (_entityById.ContainsKey(locationDb.Id))
            {
                throw new EntityAlreadyExistsExeption(ErrorMessages.ENTITY_ALREADY_EXISTS + locationDb.Id);
            }

            if (ExistsByPath(locationDb.Path))
            {
                throw new EntityAlreadyExistsExeption($"Location with path '{locationDb.Path}' already exists");
            }

            return base.AddAsync(locationDb, cancellationToken);
        }

        public Task<LocationDb?> FindByPathAsync(string path, CancellationToken cancellationToken)
        {
            if(string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));

            var item = _entityById.FirstOrDefault(item => item.Value.Path == path);

            return Task.FromResult(item.Value ?? null);
        }

        public Task<bool> ExistsByPathAsync(string path, CancellationToken cancellationToken)
        {
            if(string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));

            return Task.FromResult(ExistsByPath(path));
        }

        private bool ExistsByPath(string path)
        {
            return _entityById.Values.Any(item => item.Path == path);
        }
    }
}
