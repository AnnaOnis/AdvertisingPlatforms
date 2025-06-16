using AdvertisingPlatforms.DAL.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace AdvertisingPlatforms.DAL.Repositories.DataBase
{
    public class EFRepository<TEntity> : IRepository<TEntity> where TEntity : class, IEntityDb
    {
        private readonly AdvertisingPlatformsDbContext _dbContext;
        protected DbSet<TEntity> Entities;

        public EFRepository(AdvertisingPlatformsDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            Entities = _dbContext.Set<TEntity>();
        }

        public virtual async Task<TEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await Entities.FirstAsync(entity => entity.Id == id, cancellationToken);
        }

        public virtual async Task<IReadOnlyCollection<TEntity>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await Entities.ToListAsync(cancellationToken);
        }

        public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken)
        {
            await Entities.AddAsync(entity, cancellationToken);

        }

        public virtual async Task AddRangeAsync(IReadOnlyList<TEntity> entities, CancellationToken cancellationToken)
        {
            foreach (var entity in entities)
            {
                await Entities.AddAsync(entity, cancellationToken);
            }
        }

        public virtual async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var existingEntity = await GetByIdAsync(id, cancellationToken);
            Entities.Remove(existingEntity);
        }

        public virtual Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            return Task.CompletedTask;
        }
    }
}
