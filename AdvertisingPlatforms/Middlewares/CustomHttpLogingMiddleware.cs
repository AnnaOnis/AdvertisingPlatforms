
using System.Diagnostics;
using System.Text.Json;
using AdvertisingPlatforms.Base.Constants;
using AdvertisingPlatforms.Web.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace AdvertisingPlatforms.Web.Middlewares
{
    public class CustomHttpLogingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CustomHttpLogingMiddleware> _logger;
        private readonly LoggingSettings _settings;

        public CustomHttpLogingMiddleware(RequestDelegate next, 
            ILogger<CustomHttpLogingMiddleware> logger,
            IOptions<LoggingSettings> options)
        {
            _next = next;
            _logger = logger;
            _settings = options.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (ShouldSkipLogging(context))
            {
                await _next(context);
                return;
            }

            var stopwatch = Stopwatch.StartNew();
            string traceId = context.TraceIdentifier;
            var timestamp = DateTimeOffset.UtcNow;
            var requestLog = await CreateRequestLogAsync(context);

            var originalBodyStream = context.Response.Body;
            using var responseBuffer = new MemoryStream();
            context.Response.Body = responseBuffer;

            await _next(context);

            responseBuffer.Seek(0, SeekOrigin.Begin);
            var responseLog = await CreateResponseLogAsync(context, responseBuffer);
            var durationMs = stopwatch.ElapsedMilliseconds;

            responseBuffer.Seek(0, SeekOrigin.Begin);
            await responseBuffer.CopyToAsync(originalBodyStream);

            var httpLog = new HttpLog(
                TraceId: traceId,
                Timestamp: timestamp,
                Request: requestLog,
                Response: responseLog,
                DurationMs: durationMs
            );

            _logger.LogInformation("HTTP Log:\n{JsonLog}", HttpLogToJson(httpLog));
        }

        private bool ShouldSkipLogging(HttpContext context)
        {
            var path = context.Request.Path.Value ?? string.Empty;
            return _settings.ExcludePaths.Any(p => path.StartsWith(p));
        }

        private async Task<RequestLog?> CreateRequestLogAsync(HttpContext context)
        {
            if (!_settings.EnableRequestLogging) return null;

            var request = context.Request;
            request.EnableBuffering();

            foreach (var header in HttpConstants.RequestHeadersToRemove)
            {
                context.Request.Headers.Remove(header);
            }

            return new RequestLog(
                Method: request.Method,
                Protocol: request.Protocol,
                Path: request.Path,
                QueryString: request.QueryString.ToString(),
                Headers: _settings.IncludeHeaders
                    ? GetFilteredHeaders(request.Headers) : [],
                Body: await ReadAndProcessBodyAsync(
                    request.Body, 
                    request.ContentType, 
                    _settings.BodyLogLimit)
            );
        }

        private async Task<ResponseLog?> CreateResponseLogAsync(HttpContext context, Stream responseStream)
        {
            if (!_settings.EnableResponseLogging) return null;

            return new ResponseLog(
                StatusCode: context.Response.StatusCode,
                ContentType: context.Response.ContentType,
                ContentLength: responseStream.Length,
                Body: await ReadAndProcessBodyAsync(
                    stream: responseStream,
                    contentType: context.Response.ContentType,
                    maxSize: _settings.BodyLogLimit
                )
            );
        }

        private Dictionary<string, string> GetFilteredHeaders(IHeaderDictionary headers)
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

        private async Task<string?> ReadAndProcessBodyAsync(
            Stream stream,
            string? contentType,
            int maxSize)
        {
            if (stream.CanSeek) stream.Seek(0, SeekOrigin.Begin);

            if (stream.Length > maxSize)
                return $"[BODY_TOO_LARGE: {stream.Length} bytes]";

            if (!IsTextContent(contentType))
                return $"[CONTENT_IS_NOT_TEXT: {contentType}]";

            using var reader = new StreamReader(stream, leaveOpen: true);
            var content = await reader.ReadToEndAsync();

            if (stream.CanSeek) stream.Seek(0, SeekOrigin.Begin);

            return content;
        }

        private bool IsTextContent(string? contentType)
        {
            if (string.IsNullOrEmpty(contentType)) return false;

            var textTypes = new[]
            {
                "application/json",
                "text/plain",
                "application/xml",
            };

            return textTypes.Any(t => contentType.Contains(t));
        }

        private string HttpLogToJson(HttpLog httpLog)
        {
            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            string jsonLog = JsonSerializer.Serialize(httpLog, jsonOptions);
            return jsonLog;
        }
    }
}
