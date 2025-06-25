using AdvertisingPlatforms.DAL.Abstractions;

namespace AdvertisingPlatforms.DAL.Repositories.DataBase
{
    public class UnitOfWorkEF : IUnitOfWork
    {
        private readonly AdvertisingPlatformsDbContext _dbContext;
        private readonly IAdvertisementRepository _advertisementRepository;
        private readonly IAdvertisingPlatformRepository _advertisingPlatformRepository;
        private readonly ILocationRepository _locationRepository;

        public IAdvertisementRepository AdvertisementRepository => _advertisementRepository;
        public IAdvertisingPlatformRepository AdvertisingPlatformRepository => _advertisingPlatformRepository;
        public ILocationRepository LocationRepository => _locationRepository;
  

        public UnitOfWorkEF(AdvertisingPlatformsDbContext dbContext, 
            IAdvertisementRepository advertisementRepository,
            IAdvertisingPlatformRepository advertisingPlatformRepository,
            ILocationRepository locationRepository)
        {
            _dbContext = dbContext;
            _advertisementRepository = advertisementRepository;
            _advertisingPlatformRepository = advertisingPlatformRepository;
            _locationRepository = locationRepository;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
