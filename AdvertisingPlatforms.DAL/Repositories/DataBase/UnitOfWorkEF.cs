using System.Reflection.Metadata.Ecma335;
using AdvertisingPlatforms.DAL.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace AdvertisingPlatforms.DAL.Repositories.DataBase
{
    public class UnitOfWorkEF : IUnitOfWork
    {
        private readonly AdvertisingPlatformsDbContext _dbContext;
        private readonly IDbContextTransaction _transaction;
        private bool _disposed;

        public UnitOfWorkEF(AdvertisingPlatformsDbContext dbContext)
        {
            _dbContext = dbContext;
            _transaction = _dbContext.Database.BeginTransaction();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public async Task Commit()
        {
            await _transaction.CommitAsync();
        }

        public async Task RollBack()
        {
            await _transaction.RollbackAsync();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _dbContext.Dispose();
                _transaction?.Dispose();
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}
