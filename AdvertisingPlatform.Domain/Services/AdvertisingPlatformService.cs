using Microsoft.Extensions.Logging;
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
        private readonly ILogger<AdvertisingPlatformService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDomainModelFactory<AdvertisingPlatformDb, AdvertisingPlatform> _factory;

        public AdvertisingPlatformService(ILogger<AdvertisingPlatformService> logger, 
            IUnitOfWork unitOfWork,
            IDomainModelFactory<AdvertisingPlatformDb, AdvertisingPlatform> factory)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _factory = factory;
        }

        public async Task<AdvertisingPlatform> GetById(Guid platformId, CancellationToken cancellationToken)
        {
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

        public async Task AddPlatform(AdvertisingPlatform platform, CancellationToken cancellationToken)
        {
            var platformDb = new AdvertisingPlatformDb(platform.AdvertisementId, platform.LocationId);
            await _unitOfWork.AdvertisingPlatformRepository.AddAsync(platformDb, cancellationToken);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdatePlatform(AdvertisingPlatform platform, CancellationToken cancellationToken)
        {
            var platformDb = await _unitOfWork.AdvertisingPlatformRepository.GetByIdAsync(platform.Id, cancellationToken);
            platformDb.AdvertisementId = platform.AdvertisementId;
            platformDb.LocationId = platform.LocationId;
            await _unitOfWork.AdvertisingPlatformRepository.UpdateAsync(platformDb, cancellationToken);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeletePlatform(Guid platformId, CancellationToken cancellationToken)
        {
            await _unitOfWork.AdvertisingPlatformRepository.DeleteAsync(platformId, cancellationToken);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IReadOnlyCollection<AdvertisingPlatform>> Search(string locationPath, 
            CancellationToken cancellationToken, 
            string? sortBy, 
            bool isAsc)
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
