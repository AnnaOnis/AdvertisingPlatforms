using AdvertisingPlatforms.Base.Constants;

namespace AdvertisingPlatforms.Web.Middlewares
{
    public class ModifyRequestHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public ModifyRequestHeadersMiddleware(
            RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {

            foreach (var header in HttpConstants.RequestHeadersToRemove)
            {
                context.Request.Headers.Remove(header);
            }

            await _next(context);
        }
    }
}
