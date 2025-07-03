using AdvertisingPlatforms.Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;
using AdvertisingPlatforms.Domain.Models;
using AdvertisingPlatforms.Web.HttpModels.Requests;
using AdvertisingPlatforms.Base.Extensions;
using AutoMapper;
using AdvertisingPlatforms.DAL.Entities;

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
        private readonly IMapper _mapper;

        public LocationsController(ILocationService service,
            IMapper mapper)
        {
            _locationService = service;
            _mapper = mapper;
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
            var response = _mapper.Map<Location[]>(locations);
            return Ok(response);
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
            var response = _mapper.Map<Location>(location);
            return Ok(response);
        }

        /// <summary>
        /// Find location by path
        /// </summary>
        /// <param name="path">Location path</param>
        /// <response code="200">Success response</response>
        /// <response code="404">Location not found</response>
        [HttpGet("[action]")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Location>> FindLocationByPath([FromQuery] string path, CancellationToken cancellationToken)
        {
            var normalizePath = path.NormalizeLocationPath();
            var location = await _locationService.FindByPath(normalizePath, cancellationToken);
            if (location == null)
                return NotFound();

            var response = _mapper.Map<Location>(location);
            return Ok(response);
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
            [FromBody] CreateLocationRequest request, 
            CancellationToken cancellationToken)
        {
            var createdLocation = await _locationService.CreateLocation(request.Path, request.ParentId, cancellationToken);

            var response = _mapper.Map<Location>(createdLocation);
            return Ok(response);
        }

        /// <summary>
        /// Update existing location
        /// </summary>
        /// <param name="id">Location ID</param>
        /// <param name="request">Updated location data</param>
        /// <response code="200">Location updated successfully</response>
        /// <response code="400">Invalid location data</response>
        /// <response code="404">Location not found</response>
        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> UpdateLocation(
            [FromBody] UpdateLocationRequest request, 
            CancellationToken cancellationToken)
        {
            await _locationService.UpdateLocation(request.Id, request.Path, request.ParentId, cancellationToken);
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
            return Ok();
        }
    }
} 