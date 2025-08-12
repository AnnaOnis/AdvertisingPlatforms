
using AdvertisingPlatforms.DAL.Abstractions;
using AdvertisingPlatforms.DAL.Entities;

namespace AdvertisingPlatforms.DAL.Delegates
{
    public delegate IOrderedQueryable<AdvertisingPlatformDb> AdvertisingPlatformsSortDelegate(IQueryable<AdvertisingPlatformDb> entities);
}
