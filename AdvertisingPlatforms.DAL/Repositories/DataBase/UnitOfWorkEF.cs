using AdvertisingPlatforms.DAL.Abstractions;

namespace AdvertisingPlatforms.DAL.Repositories.DataBase
{
    public class UnitOfWorkEF : IUnitOfWork
    {
        private readonly AdvertisingPlatformsDbContext _dbContext;
        private readonly IAdvertisementRepository _advertisementRepository;
        private readonly IAdvertisingPlatformRepository _advertisingPlatformRepository;
        private readonly ILocationRepository _locationRepository;

        public IAdvertisementRepository AdvertisementRepository
        {
            get { return _advertisementRepository ?? new EFAdvertisementRepository(_dbContext); }
        }
        public IAdvertisingPlatformRepository AdvertisingPlatformRepository
        {
            get { return _advertisingPlatformRepository ?? new EFAdvertisingPlatformRepository(_dbContext); }
        }
        public ILocationRepository LocationRepository
        {
            get { return _locationRepository ?? new EFLocationRepository(_dbContext); }
        }

        public UnitOfWorkEF(AdvertisingPlatformsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
