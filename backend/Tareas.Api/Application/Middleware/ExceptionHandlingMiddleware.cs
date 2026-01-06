using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Tareas.Api.Application.Exceptions;
using Tareas.Api.Application.Models;

namespace Tareas.Api.Application.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (status, error, details) = exception switch
        {
            NotFoundException => (HttpStatusCode.NotFound, "NotFound", (object?)null),
            DependencyBlockedException dependencyBlocked => (HttpStatusCode.Conflict, "DependencyBlocked", new
            {
                dependencyBlocked.BlockingTaskIds
            }),
            DomainValidationException validation => (HttpStatusCode.BadRequest, "ValidationError", validation.Details),
            DbUpdateException dbUpdateException => (HttpStatusCode.BadRequest, "ValidationError", new
            {
                dbUpdateException.Message
            }),
            _ => (HttpStatusCode.InternalServerError, "ServerError", (object?)null)
        };

        var response = new ErrorResponse
        {
            Error = error,
            Message = exception.Message,
            Details = details
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)status;

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        return context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}
