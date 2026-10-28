using OLP.Application.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace OLP.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(
            RequestDelegate next,
            ILogger<ErrorHandlingMiddleware> logger)
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
            catch (OperationCanceledException)
                when (context.RequestAborted.IsCancellationRequested)
            {
                // USER-მა მოთხოვნა გააუქმა.
            }
            catch (Exception ex)
            {
                if (context.Response.HasStarted)
                {
                    throw;
                }

                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(
            HttpContext context,
            Exception ex)
        {
            var statusCode = ex switch
            {
                BadRequestException => StatusCodes.Status400BadRequest,
                ValidationException => StatusCodes.Status400BadRequest,
                UnauthorizedException => StatusCodes.Status401Unauthorized,
                ForbiddenException => StatusCodes.Status403Forbidden,
                NotFoundException => StatusCodes.Status404NotFound,
                ConflictException => StatusCodes.Status409Conflict,
                ExternalServiceUnavailableException
                    => StatusCodes.Status503ServiceUnavailable,

                _ => StatusCodes.Status500InternalServerError
            };

            if (statusCode >= 500)
            {
                _logger.LogError(
                    ex,
                    "Request failed. TraceId: {TraceId}",
                    context.TraceIdentifier);
            }

            var message = statusCode switch
            {
                StatusCodes.Status500InternalServerError
                    => "An unexpected error occurred.",

                StatusCodes.Status503ServiceUnavailable
                    => "The service is temporarily unavailable.",

                _ => ex.Message
            };

            var response = new CommonResponse<object>(
                message,
                null,
                false,
                statusCode)
            {
                TraceId = context.TraceIdentifier
            };

            context.Response.Clear();
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var json = JsonSerializer.Serialize(
                response,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

            await context.Response.WriteAsync(
                json,
                context.RequestAborted);
        }
    }
}
