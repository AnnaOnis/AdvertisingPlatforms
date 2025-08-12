using AdvertisingPlatforms.DAL.Abstractions;

namespace AdvertisingPlatforms.DAL.Repositories.InMemory
{
    public class UnitOfWorkInMemory : IUnitOfWork
    {
        public Task<int> SaveChangesAsync()
        {
            return Task.FromResult(1);
        }

        public Task Commit() => Task.CompletedTask;
        
        public Task RollBack() => Task.CompletedTask;

        public void Dispose()
        {
            
        }
    }
}
