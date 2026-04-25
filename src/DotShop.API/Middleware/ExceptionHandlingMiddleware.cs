// ============================================================
// ExceptionHandlingMiddleware.cs — Global Error Handler
// ============================================================
// Without this, any unhandled exception in a controller or service
// would either return a raw HTML error page (useless to Angular) or
// expose a full stack trace to the client (a security risk).
//
// This middleware sits at the very start of the pipeline and wraps
// every request in a try/catch. When an exception escapes a controller,
// we catch it here and return a clean, consistent JSON error response.
//
// EXCEPTION → HTTP STATUS CODE MAPPING:
//   KeyNotFoundException     → 404 Not Found     (e.g. product ID doesn't exist)
//   InvalidOperationException → 400 Bad Request   (e.g. out of stock, duplicate email)
//   Any other Exception       → 500 Internal Server Error
// ============================================================

namespace DotShop.API.Middleware;

public class ExceptionHandlingMiddleware
{
    // RequestDelegate is a function: HttpContext → Task.
    // It represents the "next" step in the middleware pipeline.
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next   = next;
        _logger = logger;
    }

    // InvokeAsync is called by ASP.NET Core for every HTTP request.
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Pass the request along to the next middleware / controller.
            await _next(context);
        }
        catch (KeyNotFoundException ex)
        {
            // A service threw KeyNotFoundException — the requested resource doesn't exist.
            _logger.LogWarning(ex, "Resource not found: {Message}", ex.Message);
            context.Response.StatusCode  = StatusCodes.Status404NotFound;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            // A service threw InvalidOperationException — the request was valid but
            // the operation couldn't be completed (e.g. out of stock, duplicate email).
            _logger.LogWarning(ex, "Bad request: {Message}", ex.Message);
            context.Response.StatusCode  = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            // Unexpected error — log the full exception (for the developer),
            // but return a generic message to the client (don't leak internals).
            _logger.LogError(ex, "Unhandled exception");
            context.Response.StatusCode  = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new { error = "An unexpected error occurred." });
        }
    }
}
