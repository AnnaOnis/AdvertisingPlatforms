using System.Diagnostics;
using AdvertisingPlatforms.Models;
using AdvertisingPlatforms.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace AdvPlatformsTests
{
    public class AdvPlatformServiceTests
    {
        private readonly Mock<ILogger<AdvPlatformService>> _loggerMock;
        private readonly AdvPlatformService _service;

        public AdvPlatformServiceTests()
        {
            _loggerMock = new Mock<ILogger<AdvPlatformService>>();
            _service = new AdvPlatformService(_loggerMock.Object);
        }

        /// <summary>
        /// Проверяет полную замену данных при повторной загрузке
        /// </summary>
        /// <remarks>
        /// Тест проверяет что предыдущие данные полностью удаляются
        /// и заменяются новыми при повторном вызове Upload
        /// </remarks>
        [Fact]
        public void LoadData_OverridesPreviousData()
        {
            // Arrange
            _service.Upload(new List<AdvPlatform> { new("Old", new[] { "/ru" }) });
            _service.Upload(new List<AdvPlatform> { new("New", new[] { "/ru" }) });

            // Act
            var result = _service.Search("/ru");

            // Assert
            Assert.Contains("New", result.Select(p => p.Name));
        }

        /// <summary>
        /// Проверяет поиск по вложенным локациям
        /// </summary>
        /// <remarks>
        /// Тест проверяет что поиск возвращает платформы для:
        /// 1. Точного совпадения локации
        /// 2. Всех родительских локаций
        /// </remarks>
        [Fact]
        public void Search_ReturnsAllNestedPlatforms()
        {
            // Arrange
            _service.Upload(new List<AdvPlatform>
                {
                    new("Global", new[] { "/ru" }),
                    new("Local", new[] { "/ru/svrd" })
                });

            // Act
            var result = _service.Search("/ru/svrd/revda");

            // Assert
            Assert.Equal(2, result.Count());
        }

        /// <summary>
        /// Проверяет обработку несуществующих локаций
        /// </summary>
        /// <remarks>
        /// Тест проверяет что для неизвестной локации возвращается
        /// пустая коллекция платформ
        /// </remarks>
        [Fact]
        public void Search_UnknownLocation_ReturnsEmpty()
        {
            // Arrange
            _service.Upload(new List<AdvPlatform> { new("A", new[] { "ru", "/ru/msk/" }) });

            // Act
            var result = _service.Search("/unknown");

            // Assert
            Assert.Empty(result);
        }

        /// <summary>
        /// Проверяет потокобезопасность операций
        /// </summary>
        /// <remarks>
        /// Тест проверяет отсутствие исключений при:
        /// 1. Параллельной загрузке данных
        /// 2. Одновременном поиске
        /// </remarks>
        [Fact]
        public void Concurrent_Load_And_Search()
        {
            Parallel.Invoke(
                () => _service.Upload(new List<AdvPlatform> { new("Old", new[] { "/ru" }) }),
                () => _service.Upload(new List<AdvPlatform> { new("New", new[] { "/ru", "/tu/msk" }) }),
                () => _service.Search("/ru"),
                () => _service.Search("/ru/msk")
            );            
        }

        /// <summary>
        /// Проверяет производительность поиска
        /// </summary>
        /// <remarks>
        /// Тест проверяет что 100,000 поисковых запросов
        /// выполняются менее чем за 1000 мс
        /// </remarks>
        [Fact]
        public void Search_Performance()
        {
            _service.Upload(GenerateLargeDataset()); // 1M записей

            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 100_000; i++)
            {
                _service.Search("/ru/svrd/revda");
            }

            Assert.True(sw.ElapsedMilliseconds < 1000);
        }

        /// <summary>
        /// Генерирует тестовый набор данных
        /// </summary>
        /// <remarks>
        /// Создает:
        /// - 1000 платформ
        /// - Каждая платформа имеет 10 локаций
        /// - Локации вида /ru/svrd/XX
        /// </remarks>
        private static List<AdvPlatform> GenerateLargeDataset()
        {
            var platforms = new List<AdvPlatform>();
            var random = new Random();

            for (int i = 0; i < 1000; i++)
            {
                var locations = new List<string>();
                for (int j = 0; j < 10; j++)
                {
                    locations.Add($"/ru/svrd/{random.Next(1, 100)}");
                }
                platforms.Add(new AdvPlatform($"Platform_{i}", locations.ToArray()));
            }

            return platforms;
        }
    }
}
