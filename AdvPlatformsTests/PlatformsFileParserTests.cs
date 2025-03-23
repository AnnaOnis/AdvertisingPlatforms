using System;
using System.Text;
using AdvertisingPlatforms.Parser;

namespace AdvPlatformsTests
{
    public class PlatformsFileParserTests
    {
        /// <summary>
        /// Проверяет корректность парсинга валидного файла
        /// </summary>
        /// <remarks>
        /// Тест проверяет:
        /// 1. Корректное определение количества платформ
        /// 2. Правильность парсинга названий
        /// 3. Корректность обработки локаций
        /// </remarks>
        [Fact]
        public void ParseFile_ValidFile_ReturnsPlatforms()
        {
            // Arrange
            var content = "Яндекс.Директ:/ru\nГазета уральских москвичей:/ru/msk,/ru/permobl";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

            // Act
            var platforms = PlatformsFileParser.ParseFile(stream);

            // Assert
            Assert.Equal(2, platforms.Count);
            Assert.Contains(platforms, p => p.Name == "Яндекс.Директ" && p.Locations.Contains("/ru"));
        }

        /// <summary>
        /// Проверяет обработку некорректных строк
        /// </summary>
        /// <remarks>
        /// Тест проверяет пропуск:
        /// 1. Пустых строк
        /// 2. Строк без разделителя ":"
        /// 3. Строк с пустым именем
        /// </remarks>
        [Fact]
        public void ParseFile_InvalidLines_ShouldSkip()
        {
            // Arrange
            var content = "\n\n:InvalidLine\nNoColonHere";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

            // Act
            var platforms = PlatformsFileParser.ParseFile(stream);

            // Assert
            Assert.Empty(platforms);
        }

        /// <summary>
        /// Проверяет объединение локаций для дублирующихся платформ
        /// </summary>
        /// <remarks>
        /// Тест проверяет:
        /// 1. Объединение локаций при совпадении имен
        /// 2. Устранение дубликатов локаций
        /// 3. Корректность финального списка локаций
        /// </remarks>
        [Fact]
        public void ParseFile_DuplicateLocations_ShouldMerge()
        {
            // Arrange
            var content = "Тест:/ru\nТест:/ru/msk";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

            // Act
            var platforms = PlatformsFileParser.ParseFile(stream);

            // Assert
            Assert.Single(platforms);
            var platform = platforms.First();
            Assert.Equal(2, platform.Locations.Length);
            Assert.Contains("/ru", platform.Locations);
            Assert.Contains("/ru/msk", platform.Locations);
        }
    }
}
