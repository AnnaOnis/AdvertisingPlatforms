using Microsoft.Extensions.Logging;
using AdvertisingPlatforms.Domain.Abstractions;
using AdvertisingPlatforms.DAL.Entities;
using AdvertisingPlatforms.DAL.Abstractions;
using AdvertisingPlatforms.Domain.Models;
using AdvertisingPlatforms.Base.Constants;
using AdvertisingPlatforms.Base.Exceptions;
using System.Xml.Linq;

namespace AdvertisingPlatforms.Domain.Services
{
    public class AdvertisementService : IAdvertisementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDomainModelFactory<AdvertisementDb, Advertisement> _factory;

        public AdvertisementService(
            IUnitOfWork unitOfWork,
            IDomainModelFactory<AdvertisementDb, Advertisement> factory)
        {
            _unitOfWork = unitOfWork;
            _factory = factory;
        }

        public async Task<Advertisement> GetById(Guid advertisementId, CancellationToken cancellationToken)
        {
            if (!await _unitOfWork.AdvertisementRepository.ExistsAsync(advertisementId, cancellationToken))
            {
                throw new EntityNotFoundExeption(ErrorMessages.ENTITY_NOT_FOUND + advertisementId);
            }
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

        public async Task<Advertisement> CreateAdvertisement(string advertisementName, CancellationToken cancellationToken)
        {
            if (await _unitOfWork.AdvertisementRepository.ExistsByNameAsync(advertisementName, cancellationToken))
            {
                throw new EntityAlreadyExistsExeption(ErrorMessages.ENTITY_ALREADY_EXISTS + advertisementName);
            }
            var advertisementDb = new AdvertisementDb(advertisementName);
            await _unitOfWork.AdvertisementRepository.AddAsync(advertisementDb, cancellationToken);
            await _unitOfWork.SaveChangesAsync();
            
            return _factory.Create(advertisementDb);
        }

        public async Task UpdateAdvertisement(Guid advertisementId, string newAdvertisementName, CancellationToken cancellationToken)
        {
            if (!await _unitOfWork.AdvertisementRepository.ExistsAsync(advertisementId, cancellationToken))
            {
                throw new EntityNotFoundExeption(ErrorMessages.ENTITY_NOT_FOUND + advertisementId);
            }
            if(await _unitOfWork.AdvertisementRepository.ExistsByNameAsync(newAdvertisementName, cancellationToken))
            {
                throw new EntityAlreadyExistsExeption(ErrorMessages.ENTITY_ALREADY_EXISTS + newAdvertisementName);
            }
            var advertisementDb = await _unitOfWork.AdvertisementRepository.GetByIdAsync(advertisementId, cancellationToken);
            advertisementDb.Name = newAdvertisementName;
            await _unitOfWork.AdvertisementRepository.UpdateAsync(advertisementDb, cancellationToken);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAdvertisement(Guid advertisementId, CancellationToken cancellationToken)
        {
            if (!await _unitOfWork.AdvertisementRepository.ExistsAsync(advertisementId, cancellationToken))
            {
                throw new EntityNotFoundExeption(ErrorMessages.ENTITY_NOT_FOUND + advertisementId);
            }
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