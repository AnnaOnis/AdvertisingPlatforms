using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvertisingPlatforms.Domain.Interfaces
{
    /// <summary>
    /// Валидатор сущностей
    /// </summary>
    /// <typeparam name="TEntity">Тип сущности для валидации</typeparam>
    public interface IValidator<TEntity> where TEntity : class, IEntity
    {
        /// <summary>
        /// Выполняет валидацию одиночной сущности
        /// </summary>
        /// <param name="entity">Проверяемая сущность</param>
        /// <exception cref="ArgumentNullException">Генерируется если сущность равна null</exception>
        /// <exception cref="DomainValidationException">Генерируется при нарушении бизнес-правил</exception>
        void Validate(TEntity? entity);

        /// <summary>
        /// Выполняет валидацию коллекции сущностей
        /// </summary>
        /// <param name="entity">Коллекция для проверки</param>
        /// <exception cref="ArgumentNullException">Генерируется если коллекция равна null</exception>
        /// <exception cref="DomainValidationException">Генерируется при нарушении бизнес-правил</exception>
        void Validate(IEnumerable<TEntity>? entity);
    }
}
