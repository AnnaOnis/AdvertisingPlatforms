using AdvertisingPlatforms.Base.Constants;
using AdvertisingPlatforms.Domain.Abstractions;
using AdvertisingPlatforms.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using AdvertisingPlatforms.Web.HttpModels.Requests;
using FluentValidation;
using AdvertisingPlatforms.Web.Validators;

namespace AdvertisingPlatforms.Web.Controllers
{
    public class UploadDataController : Controller
    {
        private readonly IUploadDataService _uploadDataService;
        private readonly ILogger<AdvertisingPlatformsController> _logger;

        public UploadDataController(
            IUploadDataService uploadDataService, 
            ILogger<AdvertisingPlatformsController> logger,
            IValidator<UploadFileRequest> validator)
        {
            _uploadDataService = uploadDataService;
            _logger = logger;
        }

        /// <summary>
        /// Upload platform data from file
        /// </summary>
        /// <param name="request">File upload request</param>
        /// <response code="200">Data uploaded successfully</response>
        /// <response code="400">Invalid file format</response>
        [HttpPost("upload")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> UploadData(
            [FromForm] UploadFileRequest request, 
            CancellationToken cancellationToken)
        {

            var fileData = new FormFileAdapter(request.File);

            await _uploadDataService.UploadDataFromFile(fileData, cancellationToken);

            return Ok(new
            {
                Message = LogMessages.DATA_UPLOADED_SUCCESSFULLY,
            });
        }
    }
}
