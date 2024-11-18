using CityNexus.Modulith.Domain.Modules.Shared.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CityNexus.Modulith.Api;

internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Exception occurred {Message} on {Path}", exception.Message, httpContext.Request.Path);
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Server error",
            Instance = httpContext.Request.Path,
        };
        if (exception is AppException appException)
        {
            problemDetails.Extensions.Add("traceId", httpContext.TraceIdentifier);
            problemDetails.Title = "Houve um problema";
            problemDetails.Status = appException.StatusCode;
            problemDetails.Detail = appException.Message;
            List<string> errors = [appException.Message];
            problemDetails.Extensions.Add("errors", errors);
        }
        httpContext.Response.StatusCode = problemDetails.Status.Value;
        await httpContext.Response
            .WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}