using Microsoft.Extensions.Logging;
using AdvertisingPlatforms.Domain.Abstractions;
using AdvertisingPlatforms.DAL.Entities;
using AdvertisingPlatforms.DAL.Abstractions;
using AdvertisingPlatforms.Domain.Models;
using AdvertisingPlatforms.Base.Exceptions;
using AdvertisingPlatforms.Base.Constants;
using System.IO;

namespace AdvertisingPlatforms.Domain.Services
{
    public class LocationService : ILocationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDomainModelFactory<LocationDb, Location> _factory;

        public LocationService(IUnitOfWork unitOfWork,
            IDomainModelFactory<LocationDb, Location> factory)
        {
            _unitOfWork = unitOfWork;
            _factory = factory;
        }

        public async Task<Location> GetById(Guid locationId, CancellationToken cancellationToken)
        {
            if (!await _unitOfWork.LocationRepository.ExistsAsync(locationId, cancellationToken))
            {
                throw new EntityNotFoundExñeption(ErrorMessages.ENTITY_NOT_FOUND, locationId);
            }
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

        public async Task<Location> CreateLocation(string locationPath, Guid? parentId, CancellationToken cancellationToken)
        {
            if (await _unitOfWork.LocationRepository.ExistsByPathAsync(locationPath, cancellationToken))
            {
                throw new EntityAlreadyExistsExñeption(ErrorMessages.ENTITY_ALREADY_EXISTS + locationPath);
            }
            if(parentId != null && !await _unitOfWork.LocationRepository.ExistsAsync(parentId.Value, cancellationToken))
            {
                throw new EntityNotFoundExñeption(ErrorMessages.ENTITY_NOT_FOUND, parentId);
            }
            var locationDb = new LocationDb(locationPath, parentId);
            await _unitOfWork.LocationRepository.AddAsync(locationDb, cancellationToken);
            await _unitOfWork.SaveChangesAsync();
            
            return _factory.Create(locationDb);
        }

        public async Task UpdateLocation(Guid locationId, string newLocationPath, Guid? newparentId, CancellationToken cancellationToken)
        {
            if (!await _unitOfWork.LocationRepository.ExistsAsync(locationId, cancellationToken))
            {
                throw new EntityNotFoundExñeption(ErrorMessages.ENTITY_NOT_FOUND, locationId);
            }
            if (newparentId != null && !await _unitOfWork.LocationRepository.ExistsAsync(newparentId.Value, cancellationToken))
            {
                throw new EntityNotFoundExñeption(ErrorMessages.ENTITY_NOT_FOUND, newparentId);
            }
            if (await _unitOfWork.LocationRepository.ExistsByPathAsync(newLocationPath, cancellationToken))
            {
                throw new EntityAlreadyExistsExñeption(ErrorMessages.ENTITY_ALREADY_EXISTS + newLocationPath);
            }
            var locationDb = await _unitOfWork.LocationRepository.GetByIdAsync(locationId, cancellationToken);
            locationDb.Path = newLocationPath;
            locationDb.ParentId = newparentId;
            await _unitOfWork.LocationRepository.UpdateAsync(locationDb, cancellationToken);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteLocation(Guid locationId, CancellationToken cancellationToken)
        {
            if (!await _unitOfWork.LocationRepository.ExistsAsync(locationId, cancellationToken))
            {
                throw new EntityNotFoundExñeption(ErrorMessages.ENTITY_NOT_FOUND, locationId);
            }
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