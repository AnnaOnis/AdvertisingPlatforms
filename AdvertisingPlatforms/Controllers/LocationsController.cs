using AdvertisingPlatforms.Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;
using AdvertisingPlatforms.Domain.Models;
using AdvertisingPlatforms.Web.HttpModels.Requests;

namespace AdvertisingPlatforms.Web.Controllers
{
    /// <summary>
    /// Controller for working with locations
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationService _locationService;
        private readonly ILogger<LocationsController> _logger;

        public LocationsController(ILocationService service, 
            ILogger<LocationsController> logger)
        {
            _locationService = service;
            _logger = logger;
        }

        /// <summary>
        /// Get all locations
        /// </summary>
        /// <response code="200">Success response</response>
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IReadOnlyCollection<Location>>> GetAllLocations(CancellationToken cancellationToken)
        {
            var locations = await _locationService.GetAllLocations(cancellationToken);
            return Ok(locations);
        }

        /// <summary>
        /// Get location by ID
        /// </summary>
        /// <param name="id">Location ID</param>
        /// <response code="200">Success response</response>
        /// <response code="404">Location not found</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Location>> GetLocationById(Guid id, CancellationToken cancellationToken)
        {
            var location = await _locationService.GetById(id, cancellationToken);
            return Ok(location);
        }

        /// <summary>
        /// Find location by path
        /// </summary>
        /// <param name="path">Location path</param>
        /// <response code="200">Success response</response>
        /// <response code="404">Location not found</response>
        [HttpGet("find")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Location>> FindLocationByPath([FromQuery] LocationRequest request, CancellationToken cancellationToken)
        {
            var location = await _locationService.FindByPath(request.Path, cancellationToken);
            if (location == null)
                return NotFound();
                
            return Ok(location);
        }

        /// <summary>
        /// Add new location
        /// </summary>
        /// <param name="request">Location data</param>
        /// <response code="201">Location created successfully</response>
        /// <response code="400">Invalid location data</response>
        /// <response code="409">Location already exists</response>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        public async Task<ActionResult<Location>> AddLocation(
            [FromBody] LocationRequest request, 
            CancellationToken cancellationToken)
        {
            var location = new Location(
                Guid.Empty,
                request.Path,
                request.ParentId
            );

            var createdLocation = await _locationService.AddLocation(location, cancellationToken);

            return CreatedAtAction(nameof(GetLocationById), new { id = createdLocation.Id }, createdLocation);
        }

        /// <summary>
        /// Update existing location
        /// </summary>
        /// <param name="id">Location ID</param>
        /// <param name="request">Updated location data</param>
        /// <response code="200">Location updated successfully</response>
        /// <response code="400">Invalid location data</response>
        /// <response code="404">Location not found</response>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> UpdateLocation(
            Guid id,
            [FromBody] LocationRequest request, 
            CancellationToken cancellationToken)
        {
            var location = new Location(
                id,
                request.Path,
                request.ParentId
            );

            await _locationService.UpdateLocation(location, cancellationToken);

            return Ok();
        }

        /// <summary>
        /// Delete location
        /// </summary>
        /// <param name="id">Location ID</param>
        /// <response code="204">Location deleted successfully</response>
        /// <response code="404">Location not found</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> DeleteLocation(Guid id, CancellationToken cancellationToken)
        {
            await _locationService.DeleteLocation(id, cancellationToken);
            return NoContent();
        }
    }
} 