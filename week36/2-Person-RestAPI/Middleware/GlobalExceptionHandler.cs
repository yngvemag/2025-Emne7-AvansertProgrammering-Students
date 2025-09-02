using Microsoft.AspNetCore.Diagnostics;

namespace PersonRestAPI.Middleware;

public class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger = logger;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        _logger.LogError(
            exception, 
            "Could not process request on Machine {MachineName}. TracId: {TraceId}", Environment.MachineName, httpContext.TraceIdentifier);
        
        var (statusCode, title) = MapException(exception);
        await Results.Problem(
            title: title,
            statusCode: statusCode,
            extensions: new Dictionary<string, object?>()
            {
                {"tracId", httpContext.TraceIdentifier}
            }
        ).ExecuteAsync(httpContext);
        
        return true;
    }

    private static (int statusCode, string title) MapException(Exception exception)
    {
        
        // switch (exception)
        // {
        //     case ArgumentException argumentException:
        //         return (StatusCodes.Status400BadRequest, "Invalid argument given");
        //     default:
        //         return (StatusCodes.Status500InternalServerError, "We made a mistake but we are working on it!");
        // }
        
        return exception switch
        {
            ArgumentNullException argumentNullException => (StatusCodes.Status400BadRequest, "Invalid argument given"),
            _ => (StatusCodes.Status500InternalServerError, "We made a mistake but we are working on it!")
        };
        
        return (StatusCodes.Status500InternalServerError, exception.Message);
    }
}