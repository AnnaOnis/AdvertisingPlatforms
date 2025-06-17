using Microsoft.Extensions.Logging;
using AdvertisingPlatforms.Domain.Abstractions;
using AdvertisingPlatforms.DAL.Entities;
using AdvertisingPlatforms.DAL.Abstractions;
using AdvertisingPlatforms.Domain.Models;

namespace AdvertisingPlatforms.Domain.Services
{
    public class LocationService : ILocationService
    {
        private readonly ILogger<LocationService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDomainModelFactory<LocationDb, Location> _factory;

        public LocationService(ILogger<LocationService> logger, 
            IUnitOfWork unitOfWork,
            IDomainModelFactory<LocationDb, Location> factory)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _factory = factory;
        }

        public async Task<Location> GetById(Guid locationId, CancellationToken cancellationToken)
        {
            var locationDb = await _unitOfWork.LocationRepository.GetByIdAsync(locationId, cancellationToken);
            var location = _factory.Create(locationDb);
            return location;
        }

        public async Task<IReadOnlyCollection<Location>> GetAllLocations(CancellationToken cancellationToken)
        {
            var locationsDb = await _unitOfWork.LocationRepository.GetAllAsync(cancellationToken);
            var locations = _factory.CreateMany(locationsDb);
            return locations;
        }

        public async Task<Location> AddLocation(Location location, CancellationToken cancellationToken)
        {
            var locationDb = new LocationDb(location.Path, location.ParentId, null);
            await _unitOfWork.LocationRepository.AddAsync(locationDb, cancellationToken);
            await _unitOfWork.SaveChangesAsync();
            
            return _factory.Create(locationDb);
        }

        public async Task UpdateLocation(Location location, CancellationToken cancellationToken)
        {
            var locationDb = await _unitOfWork.LocationRepository.GetByIdAsync(location.Id, cancellationToken);
            locationDb.Path = location.Path;
            locationDb.ParentId = location.ParentId;
            await _unitOfWork.LocationRepository.UpdateAsync(locationDb, cancellationToken);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteLocation(Guid locationId, CancellationToken cancellationToken)
        {
            await _unitOfWork.LocationRepository.DeleteAsync(locationId, cancellationToken);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<Location?> FindByPath(string path, CancellationToken cancellationToken)
        {
            var locationDb = await _unitOfWork.LocationRepository.FindByPathAsync(path, cancellationToken);
            if (locationDb == null) return null;
            
            var location = _factory.Create(locationDb);
            return location;
        }
    }
} 