using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using FluentValidation;
using Taxi.Exceptions;
using Taxi.Logging;

namespace Taxi.Middleware;

public class ExceptionHandlingMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
    };

    private readonly RequestDelegate next;
    private readonly ILogger<ExceptionHandlingMiddleware> logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            if (context.Response.HasStarted)
            {
                logger.LogError(LogEvents.UnhandledError, ex, "Помилка після початку відправлення відповіді.");
                throw;
            }

            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        int statusCode;
        string message;
        Dictionary<string, string[]>? errors = null;

        switch (exception)
        {
            case ValidationException validationException:
                statusCode = StatusCodes.Status400BadRequest;
                message = "Помилка валідації вхідних даних.";
                errors = validationException.Errors.GroupBy(e => e.PropertyName).ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).Distinct().ToArray());
                logger.LogWarning(LogEvents.ValidationFailed, "[Валідація] {Method} {Path}: {Errors}", context.Request.Method, context.Request.Path, string.Join("; ", validationException.Errors.Select(e => $"{e.PropertyName} — {e.ErrorMessage}")));
                break;

            case BadRequestException:
                statusCode = StatusCodes.Status400BadRequest;
                message = exception.Message;
                break;

            case ForbiddenException:
                statusCode = StatusCodes.Status403Forbidden;
                message = exception.Message;
                break;

            case NotFoundException:
                statusCode = StatusCodes.Status404NotFound;
                message = exception.Message;
                break;

            case ConflictException:
                statusCode = StatusCodes.Status409Conflict;
                message = exception.Message;
                break;

            default:
                statusCode = StatusCodes.Status500InternalServerError;
                message = "Внутрішня помилка сервера.";
                logger.LogError(LogEvents.UnhandledError, exception,
                    "Необроблений виняток під час виконання запиту {Method} {Path}",
                    context.Request.Method, context.Request.Path);
                break;
        }

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json; charset=utf-8";

        var payload = new ErrorResponse(statusCode, message, errors);
        await context.Response.WriteAsync(JsonSerializer.Serialize(payload, JsonOptions));
    }

    private sealed record ErrorResponse(int Status, string Error, Dictionary<string, string[]>? Errors);
}
