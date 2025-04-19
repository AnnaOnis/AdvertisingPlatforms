using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdvertisingPlatforms.Domain.Entities;
using AdvertisingPlatforms.Domain.Interfaces;

namespace AdvertisingPlatforms.Domain.Repositories
{
    public interface IAdvertisingPlatformRepository
    {   
        Task<IReadOnlyList<AdvertisingPlatform>> FindByLocation(Location location, CancellationToken cancellationToken);
        Task Save(IReadOnlyList<AdvertisingPlatform> platforms, CancellationToken cancellationToken);
    }
}
