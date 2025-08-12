using AdvertisingPlatforms.Base.Constants;

namespace AdvertisingPlatforms.Web.Extensions
{
    public static class HttpRequestHeadersExtension
    {
        public static Dictionary<string, string> GetFilteredHeaders(this IHeaderDictionary headers)
        {
            foreach (var header in HttpConstants.RequestHeadersToRemove)
            {
                headers.Remove(header);
            }

            var result = new Dictionary<string, string>();
            foreach (var (key, value) in headers)
            {
                result[key] = value.ToString();
            }
            return result;
        }
    }
}
