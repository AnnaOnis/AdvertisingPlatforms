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
        private readonly InMemoryData _data;

        public AdvertisingPlatformRepositoryInMemory(InMemoryData data)
        {
            _data = data;
        }

        public Task<IReadOnlyList<AdvertisingPlatform>> FindByLocation(Location location, CancellationToken cancellationToken)
        {
            return _data.GetDataByLocation(location, cancellationToken);
        }

        public Task Save(IReadOnlyList<AdvertisingPlatform> platforms, CancellationToken cancellationToken)
        {
            return _data.SaveData(platforms, cancellationToken);
        }
    }
}
