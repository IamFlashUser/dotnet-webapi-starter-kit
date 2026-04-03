using FSH.Framework.Core.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Integration.Tests.Infrastructure;

/// <summary>
/// Returns detailed exception information in test environments.
/// Preserves HTTP status codes from CustomException types (same as GlobalExceptionHandler)
/// but includes the actual exception details instead of generic messages.
/// </summary>
public sealed class DetailedTestExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails
        {
            Instance = httpContext.Request.Path
        };

        int statusCode = StatusCodes.Status500InternalServerError;

        if (exception is FluentValidation.ValidationException fluentException)
        {
            statusCode = StatusCodes.Status400BadRequest;
            problemDetails.Title = "Validation error";
            problemDetails.Detail = "One or more validation errors occurred.";

            var errors = fluentException.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray());
            problemDetails.Extensions["errors"] = errors;
        }
        else if (exception is CustomException customEx)
        {
            statusCode = (int)customEx.StatusCode;
            problemDetails.Title = customEx.GetType().Name;
            problemDetails.Detail = customEx.Message;

            if (customEx.ErrorMessages is { Count: > 0 })
            {
                problemDetails.Extensions["errors"] = customEx.ErrorMessages;
            }
        }
        else
        {
            problemDetails.Title = exception.GetType().Name;
            problemDetails.Detail = exception.Message;
            if (exception.InnerException is not null)
            {
                problemDetails.Detail += $" --> {exception.InnerException.GetType().Name}: {exception.InnerException.Message}";
            }
        }

        problemDetails.Status = statusCode;
        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}
