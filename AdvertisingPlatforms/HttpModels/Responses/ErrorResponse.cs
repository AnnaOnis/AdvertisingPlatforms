namespace AdvertisingPlatforms.Web.HttpModels.Responses
{
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Detail { get; set; } = string.Empty;
    }
}
