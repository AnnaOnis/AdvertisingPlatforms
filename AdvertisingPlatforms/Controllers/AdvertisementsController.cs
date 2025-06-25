using AdvertisingPlatforms.Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;
using AdvertisingPlatforms.Domain.Models;
using AdvertisingPlatforms.Web.HttpModels.Requests;
using AutoMapper;
using AdvertisingPlatforms.Web.HttpModels.Responses;
using AdvertisingPlatforms.DAL.Entities;

namespace AdvertisingPlatforms.Web.Controllers
{
    /// <summary>
    /// Controller for working with advertisements
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AdvertisementsController : ControllerBase
    {
        private readonly IAdvertisementService _advertisementService;
        private readonly IMapper _mapper;

        public AdvertisementsController(IAdvertisementService service, IMapper mapper)
        {
            _advertisementService = service;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all advertisements
        /// </summary>
        /// <response code="200">Success response</response>
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IReadOnlyCollection<Advertisement>>> GetAllAdvertisements(CancellationToken cancellationToken)
        {
            var advertisements = await _advertisementService.GetAllAdvertisements(cancellationToken);
            var response = _mapper.Map<AdvertisementResponse[]>(advertisements);
            return Ok(response);
        }

        /// <summary>
        /// Get advertisement by ID
        /// </summary>
        /// <param name="id">Advertisement ID</param>
        /// <response code="200">Success response</response>
        /// <response code="404">Advertisement not found</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Advertisement>> GetAdvertisementById(Guid id, CancellationToken cancellationToken)
        {
            var advertisement = await _advertisementService.GetById(id, cancellationToken);
            var response = _mapper.Map<AdvertisementResponse>(advertisement);
            return Ok(response);
        }

        /// <summary>
        /// Find advertisement by name
        /// </summary>
        /// <param name="name">Advertisement name</param>
        /// <response code="200">Success response</response>
        /// <response code="404">Advertisement not found</response>
        [HttpGet("[action]")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Advertisement>> FindAdvertisementByName([FromQuery] string name, CancellationToken cancellationToken)
        {
            var advertisement = await _advertisementService.FindByName(name, cancellationToken);
            if (advertisement == null)
                return NotFound();

            var response = _mapper.Map<AdvertisementResponse>(advertisement);
            return Ok(response);
        }

        /// <summary>
        /// Add new advertisement
        /// </summary>
        /// <param name="request">Advertisement data</param>
        /// <response code="201">Advertisement created successfully</response>
        /// <response code="400">Invalid advertisement data</response>
        /// <response code="409">Advertisement already exists</response>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        public async Task<ActionResult<Advertisement>> AddAdvertisement(
            [FromBody] CreateAdvertisementRequest request, 
            CancellationToken cancellationToken)
        {
            var createdAdvertisement = await _advertisementService.CreateAdvertisement(request.Name, cancellationToken);

            var response = _mapper.Map<AdvertisementResponse>(createdAdvertisement);
            return Ok(response);
        }

        /// <summary>
        /// Update existing advertisement
        /// </summary>
        /// <param name="id">Advertisement ID</param>
        /// <param name="request">Updated advertisement data</param>
        /// <response code="200">Advertisement updated successfully</response>
        /// <response code="400">Invalid advertisement data</response>
        /// <response code="404">Advertisement not found</response>
        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> UpdateAdvertisement(
            [FromBody] UpdateAdvertisementRequest request, 
            CancellationToken cancellationToken)
        {
            await _advertisementService.UpdateAdvertisement(request.Id, request.Name, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Delete advertisement
        /// </summary>
        /// <param name="id">Advertisement ID</param>
        /// <response code="204">Advertisement deleted successfully</response>
        /// <response code="404">Advertisement not found</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> DeleteAdvertisement(Guid id, CancellationToken cancellationToken)
        {
            await _advertisementService.DeleteAdvertisement(id, cancellationToken);
            return NoContent();
        }
    }
} 