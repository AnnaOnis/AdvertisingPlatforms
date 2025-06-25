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

        public UnitOfWorkInMemory(        
            IAdvertisementRepository advertisementRepository,
            IAdvertisingPlatformRepository advertisingPlatformRepository,
            ILocationRepository locationRepository)
        {
            _advertisementRepository = advertisementRepository;
            _advertisingPlatformRepository = advertisingPlatformRepository;
            _locationRepository = locationRepository;
        }

        public Task<int> SaveChangesAsync()
        {
            return Task.FromResult(1);
        }
    }
}
