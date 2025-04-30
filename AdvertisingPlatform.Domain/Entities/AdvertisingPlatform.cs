using AdvertisingPlatforms.Domain.Interfaces;

namespace AdvertisingPlatforms.Domain.Entities
{
    /// <summary>
    /// Представляет рекламную площадку с набором локаций
    /// </summary>
    public class AdvertisingPlatform : IEntity
    {
        /// <summary>
        /// Уникальный идентификатор платформы.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Название рекламной площадки
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Массив локаций, где действует площадка
        /// </summary>
        public IReadOnlyCollection<Location> Locations { get; set; }

        /// <summary>
        /// Создает новый экземпляр рекламной площадки
        /// </summary>
        /// <param name="name">Название площадки (не может быть пустым)</param>
        /// <param name="locations">Массив локаций (минимум одна локация)</param>
        public AdvertisingPlatform( string name, IReadOnlyList<Location> locations) 
        {
            Id = Guid.NewGuid();
            Name = name;
            Locations = locations;
        }
    }
}
