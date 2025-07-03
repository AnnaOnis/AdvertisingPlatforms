using AdvertisingPlatforms.DAL.Abstractions;
using AdvertisingPlatforms.DAL.Entities;
using AdvertisingPlatforms.Domain.Models;

namespace AdvertisingPlatforms.Domain.Abstractions
{
    public interface IDomainModelFactory<TEntityDb, TDomainModel> where TEntityDb : class, IEntity where TDomainModel : class, IDomainModel
    {
        TDomainModel Create(TEntityDb entityDb);
        IReadOnlyCollection<TDomainModel> CreateMany(IReadOnlyCollection<TEntityDb> entityDbs);
    }
}
