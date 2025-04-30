using System.Xml.Linq;
using AdvertisingPlatforms.Domain.Entities;
using AdvertisingPlatforms.Domain.Extensions;
using AdvertisingPlatforms.Domain.Interfaces;
using AdvertisingPlatforms.Domain.Validators;
using AdvertisingPlatforms.Parser;
using AdvertisingPlatforms.Services;
using AdvertisingPlatforms.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdvertisingPlatforms.Controllers
{
    /// <summary>
    /// Контроллер для работы с рекламными площадками
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AdvertisingPlatformsController : ControllerBase
    {
        private readonly IAdvertisingPlatformService _advertisingPlatformService;
        private readonly IAdvertisingPlatformParser _advertisingPlatformParser;
        private readonly ILogger<AdvertisingPlatformsController> _logger;

        public AdvertisingPlatformsController(IAdvertisingPlatformService service, 
            IAdvertisingPlatformParser parser,
            ILogger<AdvertisingPlatformsController> logger)
        {
            _advertisingPlatformService = service;
            _advertisingPlatformParser = parser;
            _logger = logger;
        }

        /// <summary>
        /// Поиск площадок по локации
        /// </summary>
        /// <param name="location">Локация в формате /регион/город</param>
        /// <response code="200">Успешный ответ</response>
        /// <response code="400">Некорректная локация</response>
        /// <response code="404">Площадки не найдены</response>
        [HttpGet("search")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IReadOnlyList<string>>> GetPlatformsByLocation([FromQuery] string locationPath, CancellationToken cancellationToken)
        {   
             var location = new Location(locationPath);

             _logger.LogInformation("Search request for location: {Location}", location.Path);

             var platforms = await _advertisingPlatformService.Search(location, cancellationToken);

             _logger.LogInformation("Returning {Count} platforms for location: {Location}",
             platforms.Count(), location.Path);

             return Ok(platforms.Select(p => p.Name));
        }

        /// <summary>
        /// Загрузка данных о площадках из файла
        /// </summary>
        /// <param name="file">Текстовый файл в формате .txt</param>
        /// <response code="200">Данные успешно загружены</response>
        /// <response code="400">Некорректный файл</response>
        [HttpPost("upload")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> UploadData(IFormFile file, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting file upload: {FileName}", file?.FileName);

            if (!file.IsValidTextFile(out var validationError))
            {
                _logger.LogWarning("File validation failed: {Error}", validationError);
                return BadRequest(validationError);
            }

            await using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Position = 0;

            _logger.LogDebug("Parsing file content");
            var platforms = _advertisingPlatformParser.ParseFile(stream);

            _logger.LogInformation("Uploading {Count} platforms", platforms.Count);
            await _advertisingPlatformService.Upload(platforms, cancellationToken);

            _logger.LogInformation("Data fron file {FileName} uploaded successfully.", file.FileName);
            
            return Ok(new
            {
                Message = "Data uploaded successfully",
                PlatformsCount = platforms.Count
            });
        }
    }
}
