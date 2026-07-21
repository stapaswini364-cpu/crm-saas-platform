namespace CRM.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;


    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger)
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
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(
                ex,
                "Unauthorized access attempt"
            );


            context.Response.Clear();

            context.Response.StatusCode =
                StatusCodes.Status401Unauthorized;

            context.Response.ContentType =
                "application/json";


            await context.Response.WriteAsJsonAsync(new
            {
                message = "Unauthorized access"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unhandled Exception: {Message}",
                ex.Message
            );


            context.Response.Clear();

            context.Response.StatusCode =
                StatusCodes.Status500InternalServerError;

            context.Response.ContentType =
                "application/json";


            await context.Response.WriteAsJsonAsync(new
            {
                message = "Internal server error",
                error = ex.Message,
                inner = ex.InnerException?.Message
            });
        }
    }
}