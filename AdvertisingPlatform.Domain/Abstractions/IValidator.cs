using AdvertisingPlatforms.DAL.Abstractions;
using AdvertisingPlatforms.Base.Exceptions;

namespace AdvertisingPlatforms.Domain.Abstractions
{
    /// <summary>
    /// Entity validator
    /// </summary>
    /// <typeparam name="TEntity">Entity type to validate</typeparam>
    public interface IValidator<TEntity> where TEntity : class, IEntityDb
    {
        /// <summary>
        /// Validates a single entity
        /// </summary>
        /// <param name="entity">Entity to validate</param>
        /// <exception cref="ArgumentNullException">Thrown when entity is null</exception>
        /// <exception cref="DomainValidationException">Thrown when business rules are violated</exception>
        void Validate(TEntity? entity);

        /// <summary>
        /// Validates a collection of entities
        /// </summary>
        /// <param name="entity">Collection to validate</param>
        /// <exception cref="ArgumentNullException">Thrown when collection is null</exception>
        /// <exception cref="DomainValidationException">Thrown when business rules are violated</exception>
        void Validate(IEnumerable<TEntity>? entity);
    }
}
