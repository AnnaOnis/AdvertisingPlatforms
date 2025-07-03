using AdvertisingPlatforms.DAL.Entities;
using AdvertisingPlatforms.Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;
using AdvertisingPlatforms.Domain.Models;
using AdvertisingPlatforms.Base.Extensions;
using AdvertisingPlatforms.Web.HttpModels.Requests;
using AutoMapper;
using AdvertisingPlatforms.Web.HttpModels.Responses;
using System;

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
        private readonly IMapper _mapper;

        public AdvertisingPlatformsController(IAdvertisingPlatformService service, IMapper mapper)
        {
            _advertisingPlatformService = service;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all platforms
        /// </summary>
        /// <response code="200">Success response</response>
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IReadOnlyCollection<AdvertisingPlatform>>> GetAllPlatforms(CancellationToken cancellationToken)
        {
            var platforms = await _advertisingPlatformService.GetAllPlatforms(cancellationToken);
            var response = _mapper.Map<AdvertisingPlatformResponse[]>(platforms);
            return Ok(response);
        }

        /// <summary>
        /// Get platform by ID
        /// </summary>
        /// <param name="id">Platform ID</param>
        /// <response code="200">Success response</response>
        /// <response code="404">Platform not found</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<AdvertisingPlatform>> GetPlatformById(Guid id, CancellationToken cancellationToken)
        {
            var platform = await _advertisingPlatformService.GetById(id, cancellationToken);
            var response = _mapper.Map<AdvertisingPlatformResponse>(platform);
            return Ok(response);
        }

        /// <summary>
        /// Search platforms by location
        /// </summary>
        /// <param name="request">Search parameters</param>
        /// <response code="200">Success response</response>
        /// <response code="400">Invalid request parameters</response>
        [HttpPost("[action]")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<IReadOnlyList<AdvertisingPlatform>>> FindPlatformsByLocationPath(
            [FromBody] SearchPlatformsRequest request,
            CancellationToken cancellationToken)
        { 
            var platforms = await _advertisingPlatformService.FindByLocation(
                request.LocationPath.NormalizeLocationPath(), 
                cancellationToken, 
                request.SortBy, 
                request.IsAsc);
            var response = _mapper.Map<AdvertisingPlatformResponse[]>(platforms);
            return Ok(response);
        }

        /// <summary>
        /// Add new platform
        /// </summary>
        /// <param name="request">Platform data</param>
        /// <response code="201">Platform created successfully</response>
        /// <response code="400">Invalid platform data</response>
        /// <response code="409">Platform already exists</response>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        public async Task<ActionResult<AdvertisingPlatform>> AddPlatform(
            [FromBody] CreateAdvertisingPlatformRequest request, 
            CancellationToken cancellationToken)
        {
            var createdPlatform = await _advertisingPlatformService.CreatePlatform(request.AdvertisementId, request.LocationId, cancellationToken);

            var response = _mapper.Map<AdvertisingPlatformResponse>(createdPlatform);
            return Ok(response);
        }

        /// <summary>
        /// Update existing platform
        /// </summary>
        /// <param name="id">Platform ID</param>
        /// <param name="request">Updated platform data</param>
        /// <response code="200">Platform updated successfully</response>
        /// <response code="400">Invalid platform data</response>
        /// <response code="404">Platform not found</response>
        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> UpdatePlatform(
            [FromBody] UpdateAdvertisingPlatformRequest request, 
            CancellationToken cancellationToken)
        {
            await _advertisingPlatformService.UpdatePlatform(request.Id,
                request.AdvertisementId,
                request.LocationId, 
                cancellationToken);

            return Ok();
        }

        /// <summary>
        /// Delete platform
        /// </summary>
        /// <param name="id">Platform ID</param>
        /// <response code="204">Platform deleted successfully</response>
        /// <response code="404">Platform not found</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> DeletePlatform(Guid id, CancellationToken cancellationToken)
        {
            await _advertisingPlatformService.DeletePlatform(id, cancellationToken);
            return Ok();
        }
    }
}
