using AdvertisingPlatforms.DAL.Entities;
using AdvertisingPlatforms.Domain.Abstractions;
using AdvertisingPlatforms.DAL.Extensions;
using Microsoft.AspNetCore.Mvc;
using AdvertisingPlatforms.Base.Constants;

namespace AdvertisingPlatforms.Web.Controllers
{
    /// <summary>
    /// Controller for working with advertising platforms
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AdvertisingPlatformsController : ControllerBase
    {
        private readonly IAdvertisingPlatformService _advertisingPlatformService;
        private readonly ILogger<AdvertisingPlatformsController> _logger;

        public AdvertisingPlatformsController(IAdvertisingPlatformService service, 
            ILogger<AdvertisingPlatformsController> logger)
        {
            _advertisingPlatformService = service;
            _logger = logger;
        }

        /// <summary>
        /// Search platforms by location
        /// </summary>
        /// <param name="locationPath">Location in /region/city format</param>
        /// <response code="200">Success response</response>
        /// <response code="400">Invalid location format</response>
        [HttpGet("search")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<IReadOnlyList<string>>> GetPlatformsByLocation([FromQuery] string locationPath, CancellationToken cancellationToken)
        {
             var location = new Location(locationPath);

             _logger.LogInformation(LogMessages.SEARCH_REQUEST_FOR_LOCATION, location.Path);

             var platforms = await _advertisingPlatformService.Search(location, cancellationToken);

             _logger.LogInformation(LogMessages.RETURNING_PLATFORMS_FOR_LOCATION,
             platforms.Count(), location.Path);

             return Ok(platforms.Select(p => p.Advertisement.Name));
        }

        /// <summary>
        /// Upload platform data from file
        /// </summary>
        /// <param name="file">Text file in .txt format</param>
        /// <response code="200">Data uploaded successfully</response>
        /// <response code="400">Invalid file format</response>
        [HttpPost("upload")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> UploadData(IFormFile file, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LogMessages.STARTING_FILE_UPLOAD, file?.FileName);

            if (!file.IsValidTextFile(out var validationError))
            {
                _logger.LogWarning(ErrorMessages.FILE_VALIDATION_FAILED, validationError);
                return BadRequest(validationError);
            }

            await using var stream = await file.FileToMemoryStreamAsync(cancellationToken);

            var platformsCount = await _advertisingPlatformService.UploadFromStream(stream, cancellationToken);

            _logger.LogInformation(LogMessages.DATA_UPLOADED_SUCCESSFULLY);
            
            return Ok(new
            {
                Message = LogMessages.DATA_UPLOADED_SUCCESSFULLY,
                PlatformsCount = platformsCount
            });
        }
    }
}
