using AdvertisingPlatforms.Domain.Abstractions;
using AdvertisingPlatforms.DAL.Entities;
using AdvertisingPlatforms.DAL.Abstractions;
using AdvertisingPlatforms.DAL.Delegates;
using AdvertisingPlatforms.Domain.Models;
using AdvertisingPlatforms.Base.Exceptions;
using AdvertisingPlatforms.Base.Constants;

namespace AdvertisingPlatforms.Domain.Services
{
    public class AdvertisingPlatformService : IAdvertisingPlatformService
    {
        private readonly IDomainModelFactory<AdvertisingPlatformDb, AdvertisingPlatform> _factory;
        private readonly IAdvertisingPlatformRepository _advertisingPlatformRepository;
        private readonly IAdvertisementRepository _advertisementRepository;
        private readonly ILocationRepository _locationRepository;

        public AdvertisingPlatformService(
            IDomainModelFactory<AdvertisingPlatformDb, AdvertisingPlatform> factory,
            IAdvertisingPlatformRepository advertisingPlatformRepository,
            IAdvertisementRepository advertisementRepository,
            ILocationRepository locationRepository)
        {
            _factory = factory;
            _advertisingPlatformRepository = advertisingPlatformRepository;
            _advertisementRepository = advertisementRepository;
            _locationRepository = locationRepository;
        }

        public async Task<AdvertisingPlatform> GetById(Guid platformId, CancellationToken cancellationToken)
        {
            if (!await _advertisingPlatformRepository.Exists(platformId, cancellationToken))
            {
                throw new EntityNotFoundException(ErrorMessages.ENTITY_NOT_FOUND, platformId);
            }
            var platformDb = await _advertisingPlatformRepository.GetById(platformId, cancellationToken);
            var platform = _factory.Create(platformDb);
            return platform;
        }

        public async Task<IReadOnlyCollection<AdvertisingPlatform>> GetAllPlatforms(CancellationToken cancellationToken)
        {
            var platformsDb = await _advertisingPlatformRepository.GetAll(cancellationToken);
            var platforms = _factory.CreateMany(platformsDb);
            return platforms;
        }

        public async Task<AdvertisingPlatform> CreatePlatform(Guid advertisementId, Guid locationId, CancellationToken cancellationToken)
        {
            if (!await _advertisementRepository.Exists(advertisementId, cancellationToken))
            {
                throw new EntityNotFoundException(ErrorMessages.ENTITY_NOT_FOUND, advertisementId);
            }
            if (!await _locationRepository.Exists(locationId, cancellationToken))
            {
                throw new EntityNotFoundException(ErrorMessages.ENTITY_NOT_FOUND, locationId);
            }
            if (await _advertisingPlatformRepository.ExistsByAdvertisementAndLocation(advertisementId, 
                locationId, 
                cancellationToken))
            {
                throw new EntityAlreadyExistsException(ErrorMessages.ENTITY_ALREADY_EXISTS);
            }
            var platformDb = new AdvertisingPlatformDb(advertisementId, locationId);
            await _advertisingPlatformRepository.Add(platformDb, cancellationToken);

            return _factory.Create(platformDb);
        }

        public async Task UpdatePlatform(Guid platformId, Guid newAdvertisementId, Guid newLocationId, CancellationToken cancellationToken)
        {
            if (!await _advertisingPlatformRepository.Exists(platformId, cancellationToken))
            {
                throw new EntityNotFoundException(ErrorMessages.ENTITY_NOT_FOUND, platformId);
            }
            if (!await _advertisementRepository.Exists(newAdvertisementId, cancellationToken))
            {
                throw new EntityNotFoundException(ErrorMessages.ENTITY_NOT_FOUND, newAdvertisementId);
            }
            if (!await _locationRepository.Exists(newLocationId, cancellationToken))
            {
                throw new EntityNotFoundException(ErrorMessages.ENTITY_NOT_FOUND, newLocationId);
            }
            var updatingPlatformDb = await _advertisingPlatformRepository.GetById(platformId, cancellationToken);
            updatingPlatformDb.AdvertisementId = newAdvertisementId;
            updatingPlatformDb.LocationId = newLocationId;
            await _advertisingPlatformRepository.Update(updatingPlatformDb, cancellationToken);
        }

        public async Task DeletePlatform(Guid platformId, CancellationToken cancellationToken)
        {
            if (!await _advertisingPlatformRepository.Exists(platformId, cancellationToken))
            {
                throw new EntityNotFoundException(ErrorMessages.ENTITY_NOT_FOUND, platformId);
            }
            await _advertisingPlatformRepository.Delete(platformId, cancellationToken);
        }

        public async Task<IReadOnlyCollection<AdvertisingPlatform>> FindByLocation(string locationPath, 
            CancellationToken cancellationToken, 
            string? sortBy = null, 
            bool isAsc = true)
        {
            var locationDb = await _locationRepository.FindByPath(locationPath, cancellationToken);
            if (locationDb != null)
            {
                var platformsDb = await _advertisingPlatformRepository.FindByLocation(locationDb, cancellationToken, GetSorter(sortBy, isAsc));
                var platforms = _factory.CreateMany(platformsDb);
                return platforms;
            }
            return Array.Empty<AdvertisingPlatform>();
        }

        private static AdvertisingPlatformsSortDelegate? GetSorter(string? sortBy, bool isAsc)
        {
            AdvertisingPlatformsSortDelegate? sorter = sortBy?.ToLower() switch
            {
                "name" => platforms => isAsc ? platforms.OrderBy(p => p.Advertisement.Name) : platforms.OrderByDescending(p => p.Advertisement.Name),
                _ => null
            };

            return sorter;
        }
    }
}
