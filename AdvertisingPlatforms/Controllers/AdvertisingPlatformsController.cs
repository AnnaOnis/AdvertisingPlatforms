using AdvertisingPlatforms.DAL.Entities;
using AdvertisingPlatforms.Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;
using AdvertisingPlatforms.Base.Constants;
using AdvertisingPlatforms.Web.Helpers;

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
             var platforms = await _advertisingPlatformService.Search(locationPath, cancellationToken);

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
            var fileData = new FormFileAdapter(file);

            var platformsCount = await _advertisingPlatformService.UploadFromFile(fileData, cancellationToken);
            
            return Ok(new
            {
                Message = LogMessages.DATA_UPLOADED_SUCCESSFULLY,
                PlatformsCount = platformsCount
            });
        }
    }
}
