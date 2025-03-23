using System.Xml.Linq;
using AdvertisingPlatforms.Parser;
using AdvertisingPlatforms.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdvertisingPlatforms.Controllers
{
    /// <summary>
    /// Контроллер для работы с рекламными площадками
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AdvPlatformsController : ControllerBase
    {
        private readonly AdvPlatformService _service;
        private readonly ILogger<AdvPlatformsController> _logger;

        public AdvPlatformsController(AdvPlatformService service, ILogger<AdvPlatformsController> logger)
        {
            _service = service;
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
        public ActionResult<IReadOnlyList<string>> GetPlatformsByLocation([FromQuery] string location)
        {
            try
            {
                _logger.LogInformation("Search request for location: {Location}", location);

                if (string.IsNullOrEmpty(location))
                {
                    _logger.LogWarning("Empty location search attempt");
                    return BadRequest("Location cannot be null or empty.");
                }

                var platforms = _service.Search(location);

                if (!platforms.Any())
                {
                    _logger.LogInformation("No platforms found for location: {Location}", location);
                    return NotFound("There are no available platforms for the specified location.");
                }

                _logger.LogInformation("Returning {Count} platforms for location: {Location}",
                platforms.Count(), location);

                return Ok(platforms.Select(p => p.Name));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching location: {Location}", location);
                return BadRequest(ex.Message);
            }
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
        public async Task<ActionResult> UploadData(IFormFile file)
        {
            const string allowedContentType = "text/plain";
            const string allowedExtension = ".txt";

            _logger.LogInformation("Starting file upload: {FileName}", file?.FileName);

            if (file is null || file.Length == 0)
            {
                _logger.LogWarning("Empty file uploaded");
                return BadRequest("File is required");
            }

            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (fileExtension != allowedExtension)
            {
                _logger.LogWarning("Invalid file extension: {FileName}", file.FileName);
                return BadRequest($"Only {allowedExtension} files are allowed");
            }

            if (!file.ContentType.Equals(allowedContentType, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Invalid content type: {ContentType}", file.ContentType);
                return BadRequest("Invalid file type");
            }

            try
            {
                await using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                stream.Position = 0;

                _logger.LogDebug("Parsing file content");
                var platforms = PlatformsFileParser.ParseFile(stream);

                if (platforms.Count == 0)
                {
                    _logger.LogWarning("Empty file content: {FileName}", file.FileName);
                    return BadRequest("The file is empty or contains incorrect data.");
                }

                _logger.LogInformation("Uploading {Count} platforms", platforms.Count);
                _service.Upload(platforms);

                _logger.LogInformation("Data fron file {FileName} uploaded successfully.", file.FileName);
                return Ok(new
                {
                    Message = "Data uploaded successfully",
                    PlatformsCount = platforms.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing file {FileName}", file.FileName);
                return BadRequest(new
                {
                    Error = "File processing error",
                    ex.Message
                });
            }

        }

    }
}
