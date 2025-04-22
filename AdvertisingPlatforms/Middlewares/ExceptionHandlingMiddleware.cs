using System.Net;
using System;
using Microsoft.AspNetCore.Mvc.Filters;
using AdvertisingPlatforms.Domain.Exceptions;
using AdvertisingPlatforms.Domain.Exceptions.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace AdvertisingPlatforms.Web.Filters
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);


            var (statusCode, type, message) = exception switch
            {
                DomainValidationException domainValidationEx =>
                    (StatusCodes.Status400BadRequest, domainValidationEx.Type, domainValidationEx.Message),
                DomainException domainEx =>
                    (StatusCodes.Status500InternalServerError, domainEx.Type, domainEx.Message),
                _ =>
                    (StatusCodes.Status500InternalServerError, "SERVER_ERROR", "Internal server error")
            };

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var errorResponse = new ProblemDetails
            {
                Status = statusCode,
                Title = type,
                Detail = message,
                Instance = context.Request.Path
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));

        }
    }
}
