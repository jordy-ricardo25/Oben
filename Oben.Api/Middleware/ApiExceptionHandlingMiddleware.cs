using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Oben.Application.Common.Exceptions;
using Oben.Domain.Exceptions;

namespace Oben.Api.Middleware;

/// <summary>
/// Convierte excepciones conocidas de dominio y aplicacion en respuestas HTTP consistentes.
/// </summary>
public sealed class ApiExceptionHandlingMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<ApiExceptionHandlingMiddleware> logger;

    /// <summary>
    /// Recibe el siguiente middleware y el logger propio de ASP.NET Core.
    /// </summary>
    public ApiExceptionHandlingMiddleware(RequestDelegate next, ILogger<ApiExceptionHandlingMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    /// <summary>
    /// Ejecuta la peticion y traduce errores esperados a ProblemDetails.
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException exception)
        {
            await WriteValidationProblemAsync(context, exception);
        }
        catch (NotFoundException exception)
        {
            await WriteProblemAsync(context, StatusCodes.Status404NotFound, "Resource not found", exception.Message);
        }
        catch (UnauthorizedException exception)
        {
            await WriteProblemAsync(context, StatusCodes.Status401Unauthorized, "Unauthorized", exception.Message);
        }
        catch (DomainException exception)
        {
            await WriteProblemAsync(context, StatusCodes.Status400BadRequest, "Domain rule violation", exception.Message);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled API error.");

            await WriteProblemAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "Unexpected error",
                "An unexpected error occurred while processing the request.");
        }
    }

    private static async Task WriteValidationProblemAsync(HttpContext context, ValidationException exception)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/problem+json";

        var errors = exception.Errors
            .GroupBy(error => error.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(error => error.ErrorMessage).ToArray());

        var problem = new ValidationProblemDetails(errors)
        {
            Title = "Validation failed",
            Status = StatusCodes.Status400BadRequest
        };

        await context.Response.WriteAsJsonAsync(problem);
    }

    private static async Task WriteProblemAsync(
        HttpContext context,
        int statusCode,
        string title,
        string detail)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails
        {
            Title = title,
            Detail = detail,
            Status = statusCode
        };

        await context.Response.WriteAsJsonAsync(problem);
    }
}
