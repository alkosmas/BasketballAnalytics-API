
using System.Net;
using System.Text.Json;
using FluentValidation; 

namespace BasketballAnalytics.Api.Middleware;

public class GlobalErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalErrorHandlingMiddleware> _logger;

    public GlobalErrorHandlingMiddleware(RequestDelegate next, ILogger<GlobalErrorHandlingMiddleware> logger){
        _next = next;
        _logger = logger;
    }

    public async Task Invoke (HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception has occured");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync (HttpContext context,Exception exception)
    {
        HttpStatusCode statusCode;
        string message;
        object? errors = null;

        switch (exception)
        {
            case ValidationException validationException:
                statusCode = HttpStatusCode.BadRequest;
                message = "One or more validation errors occurred.";
                errors = validationException.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
                break;

      /*      case NotFoundException otFoundEx:
                statusCode = HttpStatusCode.NotFound;  
                message = notFoundEx.Message;
                break;

            case UnauthorizedException unauthorizedEx:
                statusCode = HttpStatusCode.Unauthorized;  
                message = "Unauthorized access";
                break;
    */

            default:
                statusCode = HttpStatusCode.InternalServerError;
                message = "An internal server error has occurred.";
                break;

        }
        var result = JsonSerializer.Serialize(new { error = message, details = errors });
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        return context.Response.WriteAsync(result);
    }
}