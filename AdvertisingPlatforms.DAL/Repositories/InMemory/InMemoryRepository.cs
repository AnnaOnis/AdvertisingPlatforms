using System.Collections.Concurrent;
using AdvertisingPlatforms.Base.Constants;
using AdvertisingPlatforms.Base.Exceptions;
using AdvertisingPlatforms.DAL.Abstractions;

namespace AdvertisingPlatforms.DAL.Repositories.InMemory
{
    public class InMemoryRepository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
    {
        protected readonly ConcurrentDictionary<Guid, TEntity> _entityById = new();
        public InMemoryRepository() { }

        public virtual Task Add(TEntity entity, CancellationToken cancellationToken)
        {
            if(!_entityById.TryAdd(entity.Id, entity))
            {
                throw new EntityAlreadyExistsException(ErrorMessages.ENTITY_ALREADY_EXISTS +  entity.Id);
            }
            return Task.CompletedTask;
        }

        public virtual async Task AddRange(IReadOnlyList<TEntity> entities, CancellationToken cancellationToken)
        {
            foreach (var item in entities)
            {
                await Add(item, cancellationToken);
            }
        }

        public virtual async Task BulkInsert(IReadOnlyList<TEntity> entities, CancellationToken cancellationToken)
        {
            foreach (var item in entities)
            {
                await Add(item, cancellationToken);
            }
        }

        public virtual Task Delete(Guid id, CancellationToken cancellationToken)
        {
            if(!_entityById.TryRemove(id, out var entity))
            {
                throw new EntityNotFoundException(ErrorMessages.ENTITY_NOT_FOUND, id);
            }
            return Task.CompletedTask;
        }

        public virtual Task<IReadOnlyCollection<TEntity>> GetAll(CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyCollection<TEntity>>(_entityById.Values.ToList());
        }

        public virtual Task<TEntity> GetById(Guid id, CancellationToken cancellationToken)
        {
            if (!_entityById.TryGetValue(id, out var entity))
            {
                throw new EntityNotFoundException(ErrorMessages.ENTITY_NOT_FOUND, id);
            }

            return Task.FromResult(entity);
        }

        public virtual Task Update(TEntity entity, CancellationToken cancellationToken)
        {

            _entityById[entity.Id] = entity;

            return Task.CompletedTask;
        }

        public virtual Task<bool> Exists(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult(_entityById.ContainsKey(id));
        }
    }
}
