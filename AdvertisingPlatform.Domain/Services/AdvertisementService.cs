using Microsoft.Extensions.Logging;
using AdvertisingPlatforms.Domain.Abstractions;
using AdvertisingPlatforms.DAL.Entities;
using AdvertisingPlatforms.DAL.Abstractions;
using AdvertisingPlatforms.Domain.Models;

namespace AdvertisingPlatforms.Domain.Services
{
    public class AdvertisementService : IAdvertisementService
    {
        private readonly ILogger<AdvertisementService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDomainModelFactory<AdvertisementDb, Advertisement> _factory;

        public AdvertisementService(ILogger<AdvertisementService> logger, 
            IUnitOfWork unitOfWork,
            IDomainModelFactory<AdvertisementDb, Advertisement> factory)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _factory = factory;
        }

        public async Task<Advertisement> GetById(Guid advertisementId, CancellationToken cancellationToken)
        {
            var advertisementDb = await _unitOfWork.AdvertisementRepository.GetByIdAsync(advertisementId, cancellationToken);
            var advertisement = _factory.Create(advertisementDb);
            return advertisement;
        }

        public async Task<IReadOnlyCollection<Advertisement>> GetAllAdvertisements(CancellationToken cancellationToken)
        {
            var advertisementsDb = await _unitOfWork.AdvertisementRepository.GetAllAsync(cancellationToken);
            var advertisements = _factory.CreateMany(advertisementsDb);
            return advertisements;
        }

        public async Task<Advertisement> AddAdvertisement(Advertisement advertisement, CancellationToken cancellationToken)
        {
            var advertisementDb = new AdvertisementDb(advertisement.Name);
            await _unitOfWork.AdvertisementRepository.AddAsync(advertisementDb, cancellationToken);
            await _unitOfWork.SaveChangesAsync();
            
            return _factory.Create(advertisementDb);
        }

        public async Task UpdateAdvertisement(Advertisement advertisement, CancellationToken cancellationToken)
        {
            var advertisementDb = await _unitOfWork.AdvertisementRepository.GetByIdAsync(advertisement.Id, cancellationToken);
            advertisementDb.Name = advertisement.Name;
            await _unitOfWork.AdvertisementRepository.UpdateAsync(advertisementDb, cancellationToken);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAdvertisement(Guid advertisementId, CancellationToken cancellationToken)
        {
            await _unitOfWork.AdvertisementRepository.DeleteAsync(advertisementId, cancellationToken);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<Advertisement?> FindByName(string name, CancellationToken cancellationToken)
        {
            var advertisementDb = await _unitOfWork.AdvertisementRepository.FindByNameAsync(name, cancellationToken);
            if (advertisementDb == null) return null;
            
            var advertisement = _factory.Create(advertisementDb);
            return advertisement;
        }
    }
} 