using Microsoft.Extensions.Logging;
using AdvertisingPlatforms.Domain.Abstractions;
using AdvertisingPlatforms.DAL.Entities;
using AdvertisingPlatforms.DAL.Abstractions;
using AdvertisingPlatforms.DAL.Delegates;
using AdvertisingPlatforms.Domain.Models;
using AdvertisingPlatforms.Base.Exceptions;
using AdvertisingPlatforms.Base.Constants;
using System;

namespace AdvertisingPlatforms.Domain.Services
{
    public class AdvertisingPlatformService : IAdvertisingPlatformService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDomainModelFactory<AdvertisingPlatformDb, AdvertisingPlatform> _factory;

        public AdvertisingPlatformService(
            IUnitOfWork unitOfWork,
            IDomainModelFactory<AdvertisingPlatformDb, AdvertisingPlatform> factory)
        {
            _unitOfWork = unitOfWork;
            _factory = factory;
        }

        public async Task<AdvertisingPlatform> GetById(Guid platformId, CancellationToken cancellationToken)
        {
            if (!await _unitOfWork.AdvertisingPlatformRepository.ExistsAsync(platformId, cancellationToken))
            {
                throw new EntityNotFoundException(ErrorMessages.ENTITY_NOT_FOUND, platformId);
            }
            var platformDb = await _unitOfWork.AdvertisingPlatformRepository.GetByIdAsync(platformId, cancellationToken);
            var platform = _factory.Create(platformDb);
            return platform;
        }

        public async Task<IReadOnlyCollection<AdvertisingPlatform>> GetAllPlatforms(CancellationToken cancellationToken)
        {
            var platformsDb = await _unitOfWork.AdvertisingPlatformRepository.GetAllAsync(cancellationToken);
            var platforms = _factory.CreateMany(platformsDb);
            return platforms;
        }

        public async Task<AdvertisingPlatform> CreatePlatform(Guid advertisementId, Guid locationId, CancellationToken cancellationToken)
        {
            if (!await _unitOfWork.AdvertisementRepository.ExistsAsync(advertisementId, cancellationToken))
            {
                throw new EntityNotFoundException(ErrorMessages.ENTITY_NOT_FOUND, advertisementId);
            }
            if (!await _unitOfWork.LocationRepository.ExistsAsync(locationId, cancellationToken))
            {
                throw new EntityNotFoundException(ErrorMessages.ENTITY_NOT_FOUND, locationId);
            }
            if (await _unitOfWork.AdvertisingPlatformRepository.ExistsByAdvertisementAndLocationAsync(advertisementId, 
                locationId, 
                cancellationToken))
            {
                throw new EntityAlreadyExistsException(ErrorMessages.ENTITY_ALREADY_EXISTS);
            }
            var platformDb = new AdvertisingPlatformDb(advertisementId, locationId);
            await _unitOfWork.AdvertisingPlatformRepository.AddAsync(platformDb, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            return _factory.Create(platformDb);
        }

        public async Task UpdatePlatform(Guid platformId, Guid newAdvertisementId, Guid newLocationId, CancellationToken cancellationToken)
        {
            if (!await _unitOfWork.AdvertisingPlatformRepository.ExistsAsync(platformId, cancellationToken))
            {
                throw new EntityNotFoundException(ErrorMessages.ENTITY_NOT_FOUND, platformId);
            }
            if (!await _unitOfWork.AdvertisementRepository.ExistsAsync(newAdvertisementId, cancellationToken))
            {
                throw new EntityNotFoundException(ErrorMessages.ENTITY_NOT_FOUND, newAdvertisementId);
            }
            if (!await _unitOfWork.LocationRepository.ExistsAsync(newLocationId, cancellationToken))
            {
                throw new EntityNotFoundException(ErrorMessages.ENTITY_NOT_FOUND, newLocationId);
            }
            var updatingPlatformDb = await _unitOfWork.AdvertisingPlatformRepository.GetByIdAsync(platformId, cancellationToken);
            updatingPlatformDb.AdvertisementId = newAdvertisementId;
            updatingPlatformDb.LocationId = newLocationId;
            await _unitOfWork.AdvertisingPlatformRepository.UpdateAsync(updatingPlatformDb, cancellationToken);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeletePlatform(Guid platformId, CancellationToken cancellationToken)
        {
            if (!await _unitOfWork.AdvertisingPlatformRepository.ExistsAsync(platformId, cancellationToken))
            {
                throw new EntityNotFoundException(ErrorMessages.ENTITY_NOT_FOUND, platformId);
            }
            await _unitOfWork.AdvertisingPlatformRepository.DeleteAsync(platformId, cancellationToken);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IReadOnlyCollection<AdvertisingPlatform>> FindByLocation(string locationPath, 
            CancellationToken cancellationToken, 
            string? sortBy = null, 
            bool isAsc = true)
        {
            var locationDb = await _unitOfWork.LocationRepository.FindByPathAsync(locationPath, cancellationToken);
            if (locationDb != null)
            {
                var platformsDb = await _unitOfWork.AdvertisingPlatformRepository.FindByLocationAsync(locationDb, cancellationToken, GetSorter(sortBy, isAsc));
                var platforms = _factory.CreateMany(platformsDb);
                return platforms;
            }
            return Array.Empty<AdvertisingPlatform>();
        }

        private AdvertisingPlatformsSortDelegate? GetSorter(string? sortBy, bool isAsc)
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
