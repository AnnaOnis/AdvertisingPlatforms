using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvertisingPlatforms.DAL.Abstractions
{
    public interface IUnitOfWork
    {
        IAdvertisementRepository AdvertisementRepository { get; }
        IAdvertisingPlatformRepository AdvertisingPlatformRepository { get; }
        ILocationRepository LocationRepository { get; }
        Task<int> SaveChangesAsync();
    }
}
