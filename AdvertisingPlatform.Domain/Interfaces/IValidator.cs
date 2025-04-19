using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvertisingPlatforms.Domain.Interfaces
{
    public interface IValidator<TEntity> where TEntity : class, IEntity
    {
        void Validate(TEntity? entity);
        void Validate(IEnumerable<TEntity>? entity);
    }
}
