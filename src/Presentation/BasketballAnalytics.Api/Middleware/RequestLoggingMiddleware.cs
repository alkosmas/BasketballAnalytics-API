using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasketballAnalytics.Api.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next  = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = Guid.NewGuid().ToString();
            context.Items["CorrelationId"] = correlationId;

            using(_logger.BeginScope(new Dictionary<string, object>
            {
               ["CorrelationId"] = correlationId,
               ["RequestPath"] = context.Request.Path,
               ["RequestMethod"] = context.Request.Method
            }))
             {
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                _logger.LogInformation(
                    "HTTP {RequestMethod} {RequestPath} started",
                    context.Request.Method,
                    context.Request.Path);

                await _next(context);
                
                stopwatch.Stop();
                
                _logger.LogInformation(
                    "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {ElapsedMilliseconds}ms",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                
                _logger.LogError(ex,
                    "HTTP {RequestMethod} {RequestPath} failed after {ElapsedMilliseconds}ms",
                    context.Request.Method,
                    context.Request.Path,
                    stopwatch.ElapsedMilliseconds);
                
                throw;
            }
        }
    }
}

app.UseMiddleware<RequestLoggingMiddleware>();
