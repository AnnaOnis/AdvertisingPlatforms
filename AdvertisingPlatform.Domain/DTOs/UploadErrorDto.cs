
namespace AdvertisingPlatforms.Domain.DTOs
{
    public class UploadErrorDto
    {
        public string RawData { get; set; }
        public string ErrorType { get; set; }
        public string ErrorMessage { get; set; }

        public UploadErrorDto(string rawData, string errorType, string errorMessage)
        {
            RawData = rawData;
            ErrorType = errorType;
            ErrorMessage = errorMessage;
        }
    }
}
