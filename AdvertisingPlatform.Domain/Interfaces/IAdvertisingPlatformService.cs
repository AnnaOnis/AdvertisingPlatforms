

using AdvertisingPlatforms.Domain.Entities;

namespace AdvertisingPlatforms.Domain.Interfaces
{
    /// <summary>
    /// Сервис для работы с рекламными площадками
    /// </summary>
    public interface IAdvertisingPlatformService
    {
        /// <summary>
        /// Загружает новые данные о площадках
        /// </summary>
        /// <param name="platforms">Коллекция рекламных площадок</param>
        /// <param name="cancellationToken">Токен для отслеживания запросов на отмену</param>
        Task Upload(IReadOnlyList<AdvertisingPlatform> platforms, CancellationToken cancellationToken);

        /// <summary>
        /// Ищет площадки по указанной локации
        /// </summary>
        /// <param name="location">Целевая локация</param>
        /// <param name="cancellationToken">Токен для отслеживания запросов на отмену</param>
        /// <returns>Коллекция подходящих площадок</returns>
        Task<IReadOnlyCollection<AdvertisingPlatform>> Search(Location location, CancellationToken cancellationToken);
    }
}
