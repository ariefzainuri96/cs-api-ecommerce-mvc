using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Utils;

public class HttpError : ObjectResult
{
    public HttpError(string message, int statusCode = StatusCodes.Status400BadRequest)
        : base(new { error = message, status = statusCode, timestamp = DateTime.UtcNow })
    {
        StatusCode = statusCode;
    }
}
