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
    /// Controller for working with advertising platforms
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

             _logger.LogInformation("Search request for location: {Location}", location.Path);

             var platforms = await _advertisingPlatformService.Search(location, cancellationToken);

             _logger.LogInformation("Returning {Count} platforms for location: {Location}",
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
