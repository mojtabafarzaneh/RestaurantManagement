using System.Diagnostics;

namespace RestaurantManagement.Middleware;

public class DurationLoggerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<DurationLoggerMiddleware> _logger;

    public DurationLoggerMiddleware(RequestDelegate next, ILogger<DurationLoggerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();
        var requestUrl = context.Request.Path;
        try
        {
            await _next(context);
        }
        finally
        {
            var text = $"[{requestUrl}]: {sw.ElapsedMilliseconds} ms";
            _logger.LogInformation(text);
            
        }
    }
}