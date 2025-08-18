namespace AdvertisingPlatforms.Web.Logging
{
    public record HttpLog(
        string TraceId,
        DateTimeOffset Timestamp,
        RequestLog? Request,
        ResponseLog? Response,
        long DurationMs);
}
