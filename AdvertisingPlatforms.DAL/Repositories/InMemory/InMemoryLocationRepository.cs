using System;
using System.Collections.Concurrent;
using System.Xml.Linq;
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
            return Task.FromResult(_entityById.Values.FirstOrDefault(item => item.Path == path));
        }

        public Task<bool> ExistsByPathAsync(string path, CancellationToken cancellationToken)
        {
            return Task.FromResult(ExistsByPath(path));
        }

        private bool ExistsByPath(string path)
        {
            return _entityById.Values.Any(item => item.Path == path);
        }
    }
}
