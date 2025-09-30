using AdvertisingPlatforms.Domain.Abstractions;
using AdvertisingPlatforms.DAL.Entities;
using AdvertisingPlatforms.DAL.Abstractions;
using AdvertisingPlatforms.Domain.Models;
using AdvertisingPlatforms.Base.Constants;
using AdvertisingPlatforms.Base.Exceptions;

namespace AdvertisingPlatforms.Domain.Services
{
    public class AdvertisementService : IAdvertisementService
    {
        private readonly IDomainModelFactory<AdvertisementDb, Advertisement> _factory;
        private readonly IAdvertisementRepository _advertisementRepository;

        public AdvertisementService(
            IDomainModelFactory<AdvertisementDb, Advertisement> factory,
            IAdvertisementRepository advertisementRepository)
        {
            _factory = factory;
            _advertisementRepository = advertisementRepository;
        }

        public async Task<Advertisement> GetById(Guid advertisementId, CancellationToken cancellationToken)
        {
            await ValidateEntityExists(advertisementId, _advertisementRepository.Exists, cancellationToken);

            var advertisementDb = await _advertisementRepository.GetById(advertisementId, cancellationToken);
            var advertisement = _factory.Create(advertisementDb);
            return advertisement;
        }

        public async Task<IReadOnlyCollection<Advertisement>> GetAllAdvertisements(CancellationToken cancellationToken)
        {
            var advertisementsDb = await _advertisementRepository.GetAll(cancellationToken);
            var advertisements = _factory.CreateMany(advertisementsDb);
            return advertisements;
        }

        public async Task<Advertisement> CreateAdvertisement(string advertisementName, CancellationToken cancellationToken)
        {
            await ValidateEntityNotExists(advertisementName, _advertisementRepository.ExistsByName, cancellationToken);

            var advertisementDb = new AdvertisementDb(advertisementName);
            await _advertisementRepository.Add(advertisementDb, cancellationToken);
            
            return _factory.Create(advertisementDb);
        }

        public async Task UpdateAdvertisement(Guid advertisementId, string newAdvertisementName, CancellationToken cancellationToken)
        {
            await ValidateEntityExists(advertisementId, _advertisementRepository.Exists, cancellationToken);

            var advertisementDb = await _advertisementRepository.GetById(advertisementId, cancellationToken);

            if(advertisementDb.Name != newAdvertisementName)
            {
                await ValidateEntityNotExists(newAdvertisementName, _advertisementRepository.ExistsByName, cancellationToken);
            }
            
            advertisementDb.Name = newAdvertisementName;
            await _advertisementRepository.Update(advertisementDb, cancellationToken);
        }

        public async Task DeleteAdvertisement(Guid advertisementId, CancellationToken cancellationToken)
        {
            await ValidateEntityExists(advertisementId, _advertisementRepository.Exists, cancellationToken);

            await _advertisementRepository.Delete(advertisementId, cancellationToken);
        }

        public async Task<Advertisement?> FindByName(string name, CancellationToken cancellationToken)
        {
            var advertisementDb = await _advertisementRepository.FindByName(name, cancellationToken);
            if (advertisementDb == null) return null;
            
            var advertisement = _factory.Create(advertisementDb);
            return advertisement;
        }

        private async Task ValidateEntityExists(Guid entityId,
            Func<Guid, CancellationToken, Task<bool>> existenceChecker,
            CancellationToken cancellationToken)
        {
            if (!await existenceChecker(entityId, cancellationToken))
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