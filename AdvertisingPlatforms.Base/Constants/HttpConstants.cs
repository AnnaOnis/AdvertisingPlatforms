
namespace AdvertisingPlatforms.Base.Constants
{
    public class HttpConstants
    {
        public static readonly string[] RequestHeadersToRemove =
            [
                "sec-ch-ua",
                "sec-ch-ua-mobile",
                "sec-ch-ua-platform",
                "sec-fetch-site",
                "sec-fetch-mode",
                "sec-fetch-dest",
                "priority",
                "Accept-Encoding",
                "Accept-Language",
                "Origin",
                "Referer"
            ];
    }
}
