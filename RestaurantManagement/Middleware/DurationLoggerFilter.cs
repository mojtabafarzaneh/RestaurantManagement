using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RestaurantManagement.Middleware;

public class DurationLoggerFilter:IAsyncActionFilter
{
    public readonly ILogger<DurationLoggerFilter> _Logger;

    public DurationLoggerFilter(ILogger<DurationLoggerFilter> logger)
    {
        _Logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            await next();
        }
        finally
        {
            var text = $"Request Duration: {sw.ElapsedMilliseconds} ms";
            _Logger.LogInformation(text);
        }
        
    }
}