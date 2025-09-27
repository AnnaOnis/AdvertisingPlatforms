using AdvertisingPlatforms.Base.Constants;
using AdvertisingPlatforms.Base.Exceptions;
using AdvertisingPlatforms.DAL.Abstractions;
using AdvertisingPlatforms.DAL.Entities;
using AdvertisingPlatforms.Domain.Abstractions;
using AdvertisingPlatforms.Domain.Models;

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
            await ValidateEntityExists(locationId, _locationRepository.Exists, cancellationToken);
 
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
            await ValidateEntityNotExists(locationPath, _locationRepository.ExistsByPath, cancellationToken);
            if(parentId != null)
            {
                await ValidateEntityExists(parentId.Value, _locationRepository.Exists, cancellationToken);
            }
            var locationDb = new LocationDb(locationPath, parentId);
            await _locationRepository.Add(locationDb, cancellationToken);
            
            return _factory.Create(locationDb);
        }

        public async Task UpdateLocation(Guid locationId, string newLocationPath, Guid? newparentId, CancellationToken cancellationToken)
        {
            await ValidateEntityExists(locationId, _locationRepository.Exists, cancellationToken);

            var locationDb = await _locationRepository.GetById(locationId, cancellationToken);

            if(locationDb.Path != newLocationPath)
            {
                await ValidateEntityNotExists(newLocationPath, _locationRepository.ExistsByPath, cancellationToken);
            }       
            
            if (newparentId != null)
            {
                await ValidateEntityExists(newparentId.Value, _locationRepository.Exists, cancellationToken);
            }
            
            locationDb.Path = newLocationPath;
            locationDb.ParentId = newparentId;
            await _locationRepository.Update(locationDb, cancellationToken);
        }

        public async Task DeleteLocation(Guid locationId, CancellationToken cancellationToken)
        {
            await ValidateEntityExists(locationId, _locationRepository.Exists, cancellationToken);
            await _locationRepository.Delete(locationId, cancellationToken);
        }

        public async Task<Location?> FindByPath(string path, CancellationToken cancellationToken)
        {
            var locationDb = await _locationRepository.FindByPath(path, cancellationToken);
            if (locationDb == null) return null;
            
            var location = _factory.Create(locationDb);
            return location;
        }

        private async Task ValidateEntityExists(Guid entityId, 
            Func<Guid, CancellationToken, Task<bool>> existenceCheñker, 
            CancellationToken cancellationToken)
        {
            if(!await existenceCheñker(entityId, cancellationToken))
            {
                throw new EntityNotFoundException(ErrorMessages.ENTITY_NOT_FOUND, entityId);
            }  
        }

        private async Task ValidateEntityNotExists(string uniqueValue, 
            Func<string, CancellationToken, Task<bool>> existenceChecker, 
            CancellationToken cancellationToken)
        {
            if (await existenceChecker(uniqueValue, cancellationToken))
            {
                throw new EntityAlreadyExistsException(ErrorMessages.ENTITY_ALREADY_EXISTS + uniqueValue);
            }
        }
    }
} 