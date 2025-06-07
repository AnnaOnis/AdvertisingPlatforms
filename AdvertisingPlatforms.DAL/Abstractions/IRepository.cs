
namespace AdvertisingPlatforms.DAL.Abstractions
{
    public interface IRepository<TEntity> where TEntity : class, IEntity
    {
        Task<TEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<IReadOnlyCollection<TEntity>> GetAllAsync(CancellationToken cancellationToken);
        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken);
        Task<IReadOnlyCollection<TEntity>> AddRangeAsync(IReadOnlyList<TEntity> entities, CancellationToken cancellationToken);
        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken);
    }
}
