using System.Net;
using AdvertisingPlatforms.Base.Exceptions;
using System.Text.Json;
using AdvertisingPlatforms.Base.Extensions;
using AdvertisingPlatforms.Web.HttpModels.Responses;

namespace AdvertisingPlatforms.Web.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly IWebHostEnvironment _environment;
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(
            IWebHostEnvironment environment,
            RequestDelegate next)
        {
            _environment = environment;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (DomainValidationException exp)
            {
                await HandleExceptionAsync(context, exp, HttpStatusCode.BadRequest);
            }
            catch (DomainException exp)
            {
                await HandleExceptionAsync(context, exp, HttpStatusCode.BadRequest);
            }
            catch (EntityAlreadyExistsExeption exp) 
            {
                await HandleExceptionAsync(context, exp, HttpStatusCode.Conflict);
            }
            catch (EntityNotFoundExeption exp)
            {
                await HandleExceptionAsync(context, exp, HttpStatusCode.NotFound);
            }
            catch (ArgumentException exp)
            {
                await HandleExceptionAsync(context, exp, HttpStatusCode.BadRequest);
            }
            catch (AggregateException exp)
            {
                await HandleExceptionAsync(context, exp.GetBaseException(), HttpStatusCode.InternalServerError);
            }
            catch (JsonException exp)
            {
                await HandleExceptionAsync(context, exp, HttpStatusCode.BadRequest);
            }
            catch (Exception exp)
            {
                await HandleExceptionAsync(context, exp, HttpStatusCode.InternalServerError);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception, HttpStatusCode code)
        {
            var errorResponse = new ErrorResponse
            {
                StatusCode = (int)code,
                Type = code.ToString(),
                Message = exception.Message
            };


            if (!_environment.IsProduction())
                errorResponse.Detail = exception.FullMessage();

            context.Response.StatusCode = (int)code;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }
    }
}
