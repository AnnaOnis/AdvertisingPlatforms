using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdvertisingPlatforms.Domain.Entities;

namespace AdvertisingPlatforms.Domain.Interfaces
{
    /// <summary>
    /// Парсер текстовых файлов с рекламными площадками
    /// </summary>
    public interface IAdvertisingPlatformParser
    {
        /// <summary>
        /// Парсит поток данных с информацией о площадках
        /// </summary>
        /// <param name="stream">Поток данных с текстовой информацией</param>
        /// <returns>Список распарсенных площадок</returns>
        IReadOnlyList<AdvertisingPlatform> ParseFile(Stream stream);
    }
}
