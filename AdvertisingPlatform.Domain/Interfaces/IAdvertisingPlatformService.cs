using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdvertisingPlatforms.Models;

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
        void Upload(IEnumerable<AdvertisingPlatform> platforms);

        /// <summary>
        /// Ищет площадки по указанной локации
        /// </summary>
        /// <param name="location">Целевая локация</param>
        /// <returns>Коллекция подходящих площадок</returns>
        IEnumerable<AdvertisingPlatform> Search(string location);
    }
}
