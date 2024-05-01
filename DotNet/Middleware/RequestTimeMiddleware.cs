using System.Diagnostics;

namespace Endpoints.Middleware;

public class RequestTimeMiddleware : IMiddleware
{
    private readonly ILogger<RequestTimeMiddleware> _logger;

    public RequestTimeMiddleware(ILogger<RequestTimeMiddleware> logger)
    {
        _logger = logger;
    }

    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var sw = new Stopwatch();
        sw.Start();

        context.Response.OnStarting(() =>
        {
            sw.Stop();
            _logger.LogWarning("Request took: {0}ms", sw.ElapsedMilliseconds);
            return Task.CompletedTask;
        });

        return next(context);
    }
}