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
            if (!await _unitOfWork.LocationRepository.Exists(locationId, cancellationToken))
            {
                throw new EntityNotFoundException(ErrorMessages.ENTITY_NOT_FOUND, locationId);
            }
            var locationDb = await _unitOfWork.LocationRepository.GetById(locationId, cancellationToken);
            var location = _factory.Create(locationDb);
            return location;
        }

        public async Task<IReadOnlyCollection<Location>> GetAllLocations(CancellationToken cancellationToken)
        {
            var locationsDb = await _unitOfWork.LocationRepository.GetAll(cancellationToken);
            var locations = _factory.CreateMany(locationsDb);
            return locations;
        }

        public async Task<Location> CreateLocation(string locationPath, Guid? parentId, CancellationToken cancellationToken)
        {
            if (await _unitOfWork.LocationRepository.ExistsByPath(locationPath, cancellationToken))
            {
                throw new EntityAlreadyExistsException(ErrorMessages.ENTITY_ALREADY_EXISTS + locationPath);
            }
            if(parentId != null && !await _unitOfWork.LocationRepository.Exists(parentId.Value, cancellationToken))
            {
                throw new EntityNotFoundException(ErrorMessages.ENTITY_NOT_FOUND, parentId);
            }
            var locationDb = new LocationDb(locationPath, parentId);
            await _unitOfWork.LocationRepository.Add(locationDb, cancellationToken);
            await _unitOfWork.SaveChangesAsync();
            
            return _factory.Create(locationDb);
        }

        public async Task UpdateLocation(Guid locationId, string newLocationPath, Guid? newparentId, CancellationToken cancellationToken)
        {
            if (!await _unitOfWork.LocationRepository.Exists(locationId, cancellationToken))
            {
                throw new EntityNotFoundException(ErrorMessages.ENTITY_NOT_FOUND, locationId);
            }
            if (newparentId != null && !await _unitOfWork.LocationRepository.Exists(newparentId.Value, cancellationToken))
            {
                throw new EntityNotFoundException(ErrorMessages.ENTITY_NOT_FOUND, newparentId);
            }
            if (await _unitOfWork.LocationRepository.ExistsByPath(newLocationPath, cancellationToken))
            {
                throw new EntityAlreadyExistsException(ErrorMessages.ENTITY_ALREADY_EXISTS + newLocationPath);
            }
            var locationDb = await _unitOfWork.LocationRepository.GetById(locationId, cancellationToken);
            locationDb.Path = newLocationPath;
            locationDb.ParentId = newparentId;
            await _unitOfWork.LocationRepository.Update(locationDb, cancellationToken);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteLocation(Guid locationId, CancellationToken cancellationToken)
        {
            if (!await _unitOfWork.LocationRepository.Exists(locationId, cancellationToken))
            {
                throw new EntityNotFoundException(ErrorMessages.ENTITY_NOT_FOUND, locationId);
            }
            await _unitOfWork.LocationRepository.Delete(locationId, cancellationToken);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<Location?> FindByPath(string path, CancellationToken cancellationToken)
        {
            var locationDb = await _unitOfWork.LocationRepository.FindByPath(path, cancellationToken);
            if (locationDb == null) return null;
            
            var location = _factory.Create(locationDb);
            return location;
        }
    }
} 