using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;

namespace Ecommerce.GlobalException;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger = logger;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // 1. Log the exception for internal tracking
        _logger.LogError(
            exception, "An unexpected error occurred: {Message}", exception.Message);

        // 2. Determine appropriate status code and response body (Fail-Fast Logic)

        HttpStatusCode statusCode;
        string detail;

        // Gracefully handle known, user-facing exceptions (HTTP 400 Bad Request)
        if (exception is ArgumentException or InvalidPropertyException)
        {
            statusCode = HttpStatusCode.BadRequest;
            detail = exception.Message;
        }
        // Handle not found errors
        else if (exception is EntityNotFoundException)
        {
            statusCode = HttpStatusCode.NotFound;
            detail = exception.Message;
        }
        // Handle unauthorized or forbidden errors
        else if (exception is UnauthorizedAccessException)
        {
            statusCode = HttpStatusCode.Forbidden;
            detail = "Access denied.";
        }
        // Catch-all for unknown server errors (HTTP 500 Internal Server Error)
        else
        {
            statusCode = HttpStatusCode.InternalServerError;
            // Never expose sensitive server details in the production environment
            detail = "An internal server error occurred. Please try again later.";
        }

        // 3. Write the JSON response
        httpContext.Response.StatusCode = (int)statusCode;
        httpContext.Response.ContentType = "application/json";

        var response = new
        {
            Status = httpContext.Response.StatusCode,
            Title = statusCode.ToString(),
            Detail = detail,
            // Only include a unique trace ID if needed
        };

        await httpContext.Response.WriteAsync(
            JsonSerializer.Serialize(response), cancellationToken);

        // Return true to indicate the exception has been fully handled
        return true;
    }
}
