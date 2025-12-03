using Book_Lending_System.Shared.Responses;
using System.Net;
using System.Text.Json;

namespace Book_Lending_System.APIs.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

                if (!context.Response.HasStarted && context.Response.StatusCode >= 400)
                {
                    await HandleStatusCodeAsync(context, context.Response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleStatusCodeAsync(HttpContext context, int statusCode)
        {
            context.Response.ContentType = "application/json";

            string message;
            string errorType;

            switch (statusCode)
            {
                case StatusCodes.Status401Unauthorized:
                    errorType = "Unauthorized";
                    message = "Unauthorized access.";
                    break;
                case StatusCodes.Status403Forbidden:
                    errorType = "Forbidden";
                    message = "Forbidden request.";
                    break;
                case StatusCodes.Status404NotFound:
                    errorType = "NotFound";
                    message = "Resource not found.";
                    break;
                case StatusCodes.Status405MethodNotAllowed:
                    errorType = "MethodNotAllowed";
                    message = "Method not allowed.";
                    break;
                case StatusCodes.Status400BadRequest:
                    errorType = "BadRequest";
                    message = "Invalid request.";
                    break;
                default:
                    errorType = "Unexpected";
                    message = "Unexpected error occurred.";
                    break;
            }

            var apiResponse = Response<string>.Fail(
                (HttpStatusCode)statusCode,
                errorType,
                message,
                new List<string> { message },
                new
                {
                    path = context.Request.Path,
                    method = context.Request.Method,
                    timestamp = DateTime.UtcNow
                }
            );

            var result = JsonSerializer.Serialize(apiResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync(result);
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var correlationId = Guid.NewGuid().ToString();
            _logger.LogError(ex, "Unhandled exception. CorrelationId: {CorrelationId}", correlationId);

            var statusCode = HttpStatusCode.InternalServerError;
            var errorType = "Unexpected";
            var message = "An unexpected error occurred.";

            switch (ex)
            {
                case UnauthorizedAccessException:
                    statusCode = HttpStatusCode.Unauthorized;
                    errorType = "Unauthorized";
                    message = "Unauthorized access.";
                    break;

                case KeyNotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    errorType = "NotFound";
                    message = ex.Message;
                    break;

                case ArgumentException:
                    statusCode = HttpStatusCode.BadRequest;
                    errorType = "Validation";
                    message = ex.Message;
                    break;
            }

            var apiResponse = Response<string>.Fail(
                statusCode,
                errorType,
                message,
                new List<string> { ex.Message },
                new
                {
                    correlationId,
                    path = context.Request.Path,
                    method = context.Request.Method,
                    timestamp = DateTime.UtcNow
                }
            );

            var result = JsonSerializer.Serialize(apiResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;
            await context.Response.WriteAsync(result);
        }
    }

    public static class ErrorHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorHandlerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlerMiddleware>();
        }
    }
}

