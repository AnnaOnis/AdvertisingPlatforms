using AdvertisingPlatforms.DAL.Abstractions;
using Microsoft.EntityFrameworkCore;
using AdvertisingPlatforms.Base.Exceptions;
using AdvertisingPlatforms.Base.Constants;

namespace AdvertisingPlatforms.DAL.Repositories.DataBase
{
    public class EFRepository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
    {
        private readonly AdvertisingPlatformsDbContext _dbContext;
        protected DbSet<TEntity> Entities;

        public EFRepository(AdvertisingPlatformsDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            Entities = _dbContext.Set<TEntity>();
        }

        public virtual async Task<TEntity> GetById(Guid id, CancellationToken cancellationToken)
        {
            return await Entities.FirstAsync(entity => entity.Id == id, cancellationToken);
        }

        public virtual async Task<IReadOnlyCollection<TEntity>> GetAll(CancellationToken cancellationToken)
        {
            return await Entities.ToListAsync(cancellationToken);
        }

        public virtual async Task Add(TEntity entity, CancellationToken cancellationToken)
        {           
            await Entities.AddAsync(entity, cancellationToken);
        }

        public virtual async Task AddRange(IReadOnlyList<TEntity> entities, CancellationToken cancellationToken)
        {
            foreach (var entity in entities)
            {
                await Add(entity, cancellationToken);
            }
        }

        public virtual async Task Delete(Guid id, CancellationToken cancellationToken)
        {
            var existingEntity = await GetById(id, cancellationToken);
            Entities.Remove(existingEntity);
        }

        public virtual Task Update(TEntity entity, CancellationToken cancellationToken)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            return Task.CompletedTask;
        }

        public virtual async Task<bool> Exists(Guid id, CancellationToken cancellationToken)
        {
            return await Entities.AnyAsync(entity => entity.Id == id, cancellationToken);
        }
    }
}
