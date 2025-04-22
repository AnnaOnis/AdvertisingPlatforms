using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdvertisingPlatforms.Domain.Extensions;
using AdvertisingPlatforms.Domain.Interfaces;

namespace AdvertisingPlatforms.Domain.Entities
{
    /// <summary>
    /// Представляет локацию для рекламной площадки.
    /// </summary>
    public class Location : IEntity
    {
        /// <summary>
        /// Уникальный идентификатор локации.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Название локации
        /// </summary>
        public string Path { get; init; }

        /// <summary>
        /// Создает новый экземпляр локации
        /// </summary>
        /// <param name="id">Уникальный идентификатор локации</param>
        /// <param name="name">Название локации (не может быть пустым)</param>
        /// <exception cref="ArgumentException">Выбрасывается при невалидных аргументах</exception>
        public Location(string path)
        {
            Id = Guid.NewGuid();
            Path = path.NormalizeLocationPath();
        }
    }
}
