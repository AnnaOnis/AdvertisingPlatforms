namespace AdvertisingPlatforms.Web.Logging
{
    public record ResponseLog(
        int StatusCode,
        string? ContentType,
        long? ContentLength,
        string? Body);
}
