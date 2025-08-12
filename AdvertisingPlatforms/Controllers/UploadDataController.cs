using AdvertisingPlatforms.Base.Constants;
using AdvertisingPlatforms.Domain.Abstractions;
using AdvertisingPlatforms.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using AdvertisingPlatforms.Web.HttpModels.Requests;
using FluentValidation;
using AdvertisingPlatforms.Web.Validators;

namespace AdvertisingPlatforms.Web.Controllers
{
    /// <summary>
    /// Controller for uploading data to the database
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UploadDataController : Controller
    {
        private readonly IUploadDataService _uploadDataService;

        public UploadDataController(
            IUploadDataService uploadDataService, 
            IValidator<UploadFileRequest> validator)
        {
            _uploadDataService = uploadDataService;
        }

        /// <summary>
        /// Upload platform data from file
        /// </summary>
        /// <param name="request">File upload request</param>
        /// <response code="200">Data uploaded successfully</response>
        /// <response code="400">Invalid file format</response>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> UploadData(
            [FromForm] UploadFileRequest request, 
            CancellationToken cancellationToken)
        {

            var fileData = new FormFileAdapter(request.File);

            await _uploadDataService.UploadDataFromFile(fileData, cancellationToken);

            return Ok();
        }
    }
}
