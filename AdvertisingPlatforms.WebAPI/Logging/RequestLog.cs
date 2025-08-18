namespace AdvertisingPlatforms.Web.Logging
{
    public record RequestLog(
        string Method,
        string Protocol,
        string Path,
        string? QueryString,
        Dictionary<string, string> Headers,
        string? Body);
}
