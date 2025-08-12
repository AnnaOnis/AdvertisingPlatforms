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
        private readonly IDomainModelFactory<LocationDb, Location> _factory;
        private readonly ILocationRepository _locationRepository;

        public LocationService(IDomainModelFactory<LocationDb, Location> factory,
                    ILocationRepository locationRepository)
        {
            _factory = factory;
            _locationRepository = locationRepository;
        }

        public async Task<Location> GetById(Guid locationId, CancellationToken cancellationToken)
        {
            if (!await _locationRepository.Exists(locationId, cancellationToken))
            {
                throw new EntityNotFoundException(ErrorMessages.ENTITY_NOT_FOUND, locationId);
            }
            var locationDb = await _locationRepository.GetById(locationId, cancellationToken);
            var location = _factory.Create(locationDb);
            return location;
        }

        public async Task<IReadOnlyCollection<Location>> GetAllLocations(CancellationToken cancellationToken)
        {
            var locationsDb = await _locationRepository.GetAll(cancellationToken);
            var locations = _factory.CreateMany(locationsDb);
            return locations;
        }

        public async Task<Location> CreateLocation(string locationPath, Guid? parentId, CancellationToken cancellationToken)
        {
            if (await _locationRepository.ExistsByPath(locationPath, cancellationToken))
            {
                throw new EntityAlreadyExistsException(ErrorMessages.ENTITY_ALREADY_EXISTS + locationPath);
            }
            if(parentId != null && !await _locationRepository.Exists(parentId.Value, cancellationToken))
            {
                throw new EntityNotFoundException(ErrorMessages.ENTITY_NOT_FOUND, parentId);
            }
            var locationDb = new LocationDb(locationPath, parentId);
            await _locationRepository.Add(locationDb, cancellationToken);
            
            return _factory.Create(locationDb);
        }

        public async Task UpdateLocation(Guid locationId, string newLocationPath, Guid? newparentId, CancellationToken cancellationToken)
        {
            if (!await _locationRepository.Exists(locationId, cancellationToken))
            {
                throw new EntityNotFoundException(ErrorMessages.ENTITY_NOT_FOUND, locationId);
            }
            if (newparentId != null && !await _locationRepository.Exists(newparentId.Value, cancellationToken))
            {
                throw new EntityNotFoundException(ErrorMessages.ENTITY_NOT_FOUND, newparentId);
            }
            if (await _locationRepository.ExistsByPath(newLocationPath, cancellationToken))
            {
                throw new EntityAlreadyExistsException(ErrorMessages.ENTITY_ALREADY_EXISTS + newLocationPath);
            }
            var locationDb = await _locationRepository.GetById(locationId, cancellationToken);
            locationDb.Path = newLocationPath;
            locationDb.ParentId = newparentId;
            await _locationRepository.Update(locationDb, cancellationToken);
        }

        public async Task DeleteLocation(Guid locationId, CancellationToken cancellationToken)
        {
            if (!await _locationRepository.Exists(locationId, cancellationToken))
            {
                throw new EntityNotFoundException(ErrorMessages.ENTITY_NOT_FOUND, locationId);
            }
            await _locationRepository.Delete(locationId, cancellationToken);
        }

        public async Task<Location?> FindByPath(string path, CancellationToken cancellationToken)
        {
            var locationDb = await _locationRepository.FindByPath(path, cancellationToken);
            if (locationDb == null) return null;
            
            var location = _factory.Create(locationDb);
            return location;
        }
    }
} 