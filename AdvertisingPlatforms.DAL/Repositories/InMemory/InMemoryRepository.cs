using System.Collections.Concurrent;
using AdvertisingPlatforms.Base.Constants;
using AdvertisingPlatforms.Base.Exceptions;
using AdvertisingPlatforms.DAL.Abstractions;

namespace AdvertisingPlatforms.DAL.Repositories.InMemory
{
    public class InMemoryRepository<TEntity> : IRepository<TEntity> where TEntity : class, IEntityDb
    {
        protected readonly ConcurrentDictionary<Guid, TEntity> _entityById = new();
        public InMemoryRepository() { }

        public virtual Task AddAsync(TEntity entity, CancellationToken cancellationToken)
        {
            if(!_entityById.TryAdd(entity.Id, entity))
            {
                throw new EntityAlreadyExistsExeption(ErrorMessages.ENTITY_ALREADY_EXISTS +  entity.Id);
            }
            return Task.CompletedTask;
        }

        public virtual async Task AddRangeAsync(IReadOnlyList<TEntity> entities, CancellationToken cancellationToken)
        {
            foreach (var item in entities)
            {
                await AddAsync(item, cancellationToken);
            }

        }

        public virtual Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            if(!_entityById.TryRemove(id, out var entity))
            {
                throw new EntityNotFoundExeption(ErrorMessages.ENTITY_NOT_FOUND + id);
            }
            return Task.CompletedTask;
        }

        public virtual Task<IReadOnlyCollection<TEntity>> GetAllAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyCollection<TEntity>>(_entityById.Values.ToList());
        }

        public virtual Task<TEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            if (!_entityById.TryGetValue(id, out var entity))
            {
                throw new EntityNotFoundExeption(ErrorMessages.ENTITY_NOT_FOUND + id);
            }

            return Task.FromResult(entity);
        }

        public virtual Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {

            _entityById[entity.Id] = entity;

            return Task.CompletedTask;
        }

        public virtual Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult(_entityById.ContainsKey(id));
        }
    }
}
