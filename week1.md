# Week 1: Logging & Error Handling

## 🎯 Goal
Μετατρέψτε το API από "demo project" σε "production-ready" με proper observability.

---

## Part 1: Structured Logging με Serilog

### Step 1: Install Packages
```bash
cd src/Presentation/BasketballAnalytics.Api
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.Console
dotnet add package Serilog.Sinks.File
dotnet add package Serilog.Enrichers.Environment
dotnet add package Serilog.Enrichers.Thread
```

### Step 2: Configure στο Program.cs
```csharp
// ΠΡΙΝ το builder.Build()
using Serilog;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentName()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.File(
        path: "logs/basketball-api-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .CreateLogger();

try
{
    Log.Information("Starting Basketball Analytics API");
    
    var builder = WebApplication.CreateBuilder(args);
    
    // Replace default logging με Serilog
    builder.Host.UseSerilog();
    
    // ... rest of configuration
    
    var app = builder.Build();
    
    // ... middleware
    
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application failed to start");
}
finally
{
    Log.CloseAndFlush();
}
```

### Step 3: Request/Response Logging Middleware
```csharp
// Δημιούργησε: src/Presentation/BasketballAnalytics.Api/Middleware/RequestLoggingMiddleware.cs

namespace BasketballAnalytics.Api.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = Guid.NewGuid().ToString();
        context.Items["CorrelationId"] = correlationId;
        
        using (_logger.BeginScope(new Dictionary<string, object>
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

// Register στο Program.cs
app.UseMiddleware<RequestLoggingMiddleware>();
```

---

## Part 2: Result Pattern για Error Handling

### Step 1: Δημιούργησε Result Classes
```csharp
// src/Core/Application/Common/Models/Result.cs

namespace BasketballAnalytics.Application.Common.Models;

public class Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T? Value { get; }
    public Error? Error { get; }

    private Result(bool isSuccess, T? value, Error? error)
    {
        if (isSuccess && error != null)
            throw new InvalidOperationException("Success result cannot have error");
        
        if (!isSuccess && error == null)
            throw new InvalidOperationException("Failure result must have error");

        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static Result<T> Success(T value) => new(true, value, null);
    public static Result<T> Failure(Error error) => new(false, default, error);

    public TResult Match<TResult>(
        Func<T, TResult> onSuccess,
        Func<Error, TResult> onFailure)
    {
        return IsSuccess ? onSuccess(Value!) : onFailure(Error!);
    }
}

public record Error(string Code, string Message, ErrorType Type = ErrorType.Failure)
{
    public static readonly Error None = new(string.Empty, string.Empty);
}

public enum ErrorType
{
    Failure,
    Validation,
    NotFound,
    Conflict,
    Unauthorized
}
```

### Step 2: Domain Errors
```csharp
// src/Core/Domain/Errors/DomainErrors.cs

namespace BasketballAnalytics.Domain.Errors;

public static class DomainErrors
{
    public static class Team
    {
        public static Error NotFound(int teamId) => new(
            "Team.NotFound",
            $"Team with ID {teamId} was not found",
            ErrorType.NotFound);

        public static Error DuplicateName(string name) => new(
            "Team.DuplicateName",
            $"Team with name '{name}' already exists",
            ErrorType.Conflict);

        public static Error HasPlayers(int teamId, int playerCount) => new(
            "Team.HasPlayers",
            $"Cannot delete team {teamId} because it has {playerCount} active players",
            ErrorType.Conflict);
    }

    public static class Player
    {
        public static Error NotFound(int playerId) => new(
            "Player.NotFound",
            $"Player with ID {playerId} was not found",
            ErrorType.NotFound);

        public static Error InvalidAge(int age) => new(
            "Player.InvalidAge",
            $"Player age {age} is not valid. Must be between 16 and 50",
            ErrorType.Validation);

        public static Error AlreadyInTeam(int playerId, int teamId) => new(
            "Player.AlreadyInTeam",
            $"Player {playerId} is already in team {teamId}",
            ErrorType.Conflict);
    }

    public static class General
    {
        public static Error UnexpectedError => new(
            "General.UnexpectedError",
            "An unexpected error occurred",
            ErrorType.Failure);

        public static Error ValidationError(string message) => new(
            "General.ValidationError",
            message,
            ErrorType.Validation);
    }
}
```

### Step 3: Update Handler Example
```csharp
// src/Core/Application/Features/Teams/Queries/GetTeamById/GetTeamByIdQueryHandler.cs

public class GetTeamByIdQueryHandler : IRequestHandler<GetTeamByIdQuery, Result<TeamDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetTeamByIdQueryHandler> _logger;

    public GetTeamByIdQueryHandler(
        IApplicationDbContext context,
        ILogger<GetTeamByIdQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<TeamDto>> Handle(
        GetTeamByIdQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching team with ID {TeamId}", request.Id);

        var team = await _context.Teams
            .AsNoTracking()
            .Where(t => t.Id == request.Id)
            .Select(t => new TeamDto
            {
                Id = t.Id,
                Name = t.Name,
                City = t.City,
                Conference = t.Conference
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (team is null)
        {
            _logger.LogWarning("Team with ID {TeamId} not found", request.Id);
            return Result<TeamDto>.Failure(DomainErrors.Team.NotFound(request.Id));
        }

        _logger.LogInformation("Successfully retrieved team {TeamId}", request.Id);
        return Result<TeamDto>.Success(team);
    }
}
```

### Step 4: Controller με Result Handling
```csharp
// src/Presentation/BasketballAnalytics.Api/Controllers/TeamsController.cs

[ApiController]
[Route("api/[controller]")]
public class TeamsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TeamsController> _logger;

    public TeamsController(IMediator mediator, ILogger<TeamsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TeamDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var query = new GetTeamByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        return result.Match(
            onSuccess: team => Ok(team),
            onFailure: error => error.Type switch
            {
                ErrorType.NotFound => NotFound(CreateProblemDetails(error)),
                ErrorType.Validation => BadRequest(CreateProblemDetails(error)),
                ErrorType.Conflict => Conflict(CreateProblemDetails(error)),
                _ => StatusCode(500, CreateProblemDetails(error))
            });
    }

    private ProblemDetails CreateProblemDetails(Error error)
    {
        return new ProblemDetails
        {
            Title = error.Code,
            Detail = error.Message,
            Status = error.Type switch
            {
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            },
            Type = $"https://api.basketballanalytics.com/errors/{error.Code}",
            Extensions = { ["correlationId"] = HttpContext.Items["CorrelationId"] }
        };
    }
}
```

---

## Part 3: Global Exception Handler

```csharp
// src/Presentation/BasketballAnalytics.Api/Middleware/GlobalExceptionHandler.cs

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var correlationId = httpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString();

        _logger.LogError(exception,
            "Unhandled exception occurred. CorrelationId: {CorrelationId}",
            correlationId);

        var problemDetails = new ProblemDetails
        {
            Title = "An error occurred while processing your request",
            Status = StatusCodes.Status500InternalServerError,
            Type = "https://api.basketballanalytics.com/errors/internal-server-error",
            Extensions =
            {
                ["correlationId"] = correlationId,
                ["timestamp"] = DateTime.UtcNow
            }
        };

        // Σε development, δείξε λεπτομέρειες
        if (httpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
        {
            problemDetails.Detail = exception.Message;
            problemDetails.Extensions["stackTrace"] = exception.StackTrace;
        }

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}

// Register στο Program.cs
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Στο middleware pipeline
app.UseExceptionHandler();
```

---

## 🎯 Success Criteria

Μετά από αυτές τις αλλαγές, το API θα έχει:

✅ Structured logging με correlation IDs
✅ Request/Response timing
✅ Explicit error handling με Result pattern
✅ Typed domain errors
✅ Proper HTTP status codes
✅ Problem Details για errors
✅ Development vs Production error details

---

## 📝 Interview Talking Points

Όταν ρωτηθείς:

**Q: "How do you handle errors in your API?"**
**A:** "I use the Result pattern to make error handling explicit. Instead of throwing exceptions for business logic failures, I return Result<T> which forces the caller to handle both success and failure cases. Exceptions are only for truly exceptional cases. I also have typed domain errors that map to proper HTTP status codes."

**Q: "How do you monitor your application?"**
**A:** "I use Serilog for structured logging with correlation IDs for request tracing. Every request is logged with timing information. I enrich logs with environment and thread information. In production, these would go to a log aggregation service like ELK or Seq."

**Q: "What happens when an unhandled exception occurs?"**
**A:** "I have a global exception handler that catches unhandled exceptions, logs them with correlation IDs, and returns RFC 7807 Problem Details responses. In development I show stack traces, in production I hide implementation details."
