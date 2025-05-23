using System.Net;
using AdvertisingPlatforms.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using AdvertisingPlatforms.Base.Extensions;

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
            catch (DomainValidationException domainValidationEx)
            {
                await HandleExceptionAsync(context, domainValidationEx, HttpStatusCode.BadRequest);
            }
            catch (DomainException domainEx)
            {
                await HandleExceptionAsync(context, domainEx, HttpStatusCode.BadRequest);
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
            var errorResponse = new ProblemDetails
            {
                Status = (int)code,
                Title = exception.Message,
                Instance = context.Request.Path,
                Type = code.ToString()
            };


            if (!_environment.IsProduction())
                errorResponse.Detail = exception.FullMessage();

            context.Response.StatusCode = (int)code;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }
    }
}
