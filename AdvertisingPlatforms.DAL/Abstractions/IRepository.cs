namespace AdvertisingPlatforms.DAL.Abstractions
{
    public interface IRepository<TEntity> where TEntity : class, IEntity
    {
        Task<TEntity> GetById(Guid id, CancellationToken cancellationToken);
        Task<IReadOnlyCollection<TEntity>> GetAll(CancellationToken cancellationToken);
        Task Add(TEntity entity, CancellationToken cancellationToken);
        Task AddRange(IReadOnlyList<TEntity> entities, CancellationToken cancellationToken);
        Task Update(TEntity entity, CancellationToken cancellationToken);
        Task Delete(Guid id, CancellationToken cancellationToken);
        Task<bool> Exists(Guid id, CancellationToken cancellationToken);
    }
}
