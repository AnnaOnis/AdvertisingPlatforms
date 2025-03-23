namespace AdvertisingPlatforms.Models
{
    /// <summary>
    /// Представляет рекламную площадку с набором локаций
    /// </summary>
    public class AdvPlatform
    {
        /// <summary>
        /// Название рекламной площадки
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Массив локаций, где действует площадка
        /// </summary>
        public string[] Locations { get; set; }

        /// <summary>
        /// Создает новый экземпляр рекламной площадки
        /// </summary>
        /// <param name="name">Название площадки (не может быть пустым)</param>
        /// <param name="locations">Массив локаций (минимум одна локация)</param>
        /// <exception cref="ArgumentException">Выбрасывается при невалидных аргументах</exception>
        public AdvPlatform( string name, string[] locations) 
        {
            ArgumentNullException.ThrowIfNull(name);
            ArgumentNullException.ThrowIfNull(locations);

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty or whitespace.", nameof(name));

            if (locations.Length == 0)
                throw new ArgumentException("Locations array must not be empty.", nameof(locations));

            foreach (var location in locations)
            {
                if (string.IsNullOrWhiteSpace(location))
                    throw new ArgumentException("Location elements cannot be empty or whitespace.", nameof(locations));
            }

            Name = name;
            Locations = locations;
        }
    }
}
