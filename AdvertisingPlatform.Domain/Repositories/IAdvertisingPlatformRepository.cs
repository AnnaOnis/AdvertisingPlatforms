using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdvertisingPlatforms.Domain.Entities;
using AdvertisingPlatforms.Domain.Interfaces;

namespace AdvertisingPlatforms.Domain.Repositories
{
    /// <summary>
    /// Репозиторий для работы с рекламными платформами
    /// </summary>
    public interface IAdvertisingPlatformRepository
    {   
        /// <summary>
        /// Находит рекламные платформы по указанному местоположению
        /// </summary>
        /// <param name="location">Географическое местоположение для поиска</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Список рекламных платформ в указанном местоположении</returns>
        Task<IReadOnlyList<AdvertisingPlatform>> FindByLocation(Location location, CancellationToken cancellationToken);
        
        /// <summary>
        /// Сохраняет список рекламных платформ
        /// </summary>
        /// <param name="platforms">Список платформ для сохранения</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        Task Save(IReadOnlyList<AdvertisingPlatform> platforms, CancellationToken cancellationToken);
    }
}
