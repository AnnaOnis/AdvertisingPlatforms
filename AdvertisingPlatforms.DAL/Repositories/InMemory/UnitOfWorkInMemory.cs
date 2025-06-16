using AdvertisingPlatforms.DAL.Abstractions;

namespace AdvertisingPlatforms.DAL.Repositories.InMemory
{
    public class UnitOfWorkInMemory : IUnitOfWork
    {
        private readonly IAdvertisementRepository _advertisementRepository;
        private readonly IAdvertisingPlatformRepository _advertisingPlatformRepository;
        private readonly ILocationRepository _locationRepository;

        public IAdvertisementRepository AdvertisementRepository => _advertisementRepository;
        public IAdvertisingPlatformRepository AdvertisingPlatformRepository => _advertisingPlatformRepository;
        public ILocationRepository LocationRepository => _locationRepository;

        public UnitOfWorkInMemory()
        {
            _advertisementRepository = new InMemoryAdvertisementRepository();
            _advertisingPlatformRepository = new InMemoryAdvertisingPlatformRepository();
            _locationRepository = new InMemoryLocationRepository();
        }

        public Task<int> SaveChangesAsync()
        {
            return Task.FromResult(1);
        }
    }
}
