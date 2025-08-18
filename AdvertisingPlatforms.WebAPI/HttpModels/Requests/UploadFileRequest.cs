using Microsoft.AspNetCore.Http;

namespace AdvertisingPlatforms.Web.HttpModels.Requests
{
    public class UploadFileRequest
    {
        public IFormFile File { get; set; } = null!;
    }
}