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

        public Task<bool> ExistsByPathAsync(string path, CancellationToken cancellationToken)
        {
            if(string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));

            return Task.FromResult(ExistsByPath(path));
        }

        private bool ExistsByPath(string path)
        {
            return _entityById.Values.Any(item => item.Path == path);
        }
    }
}
