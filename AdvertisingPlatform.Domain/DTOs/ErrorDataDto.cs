
using AdvertisingPlatforms.Domain.Enums;

namespace AdvertisingPlatforms.Domain.DTOs
{
    public class ErrorDataDto
    {
        public string RawData { get; set; }
        public ErrorType Type { get; set; }
        public string ErrorMessage { get; set; }

        public ErrorDataDto(string rawData, ErrorType errorType, string errorMessage)
        {
            RawData = rawData;
            Type = errorType;
            ErrorMessage = errorMessage;
        }
    }
}
