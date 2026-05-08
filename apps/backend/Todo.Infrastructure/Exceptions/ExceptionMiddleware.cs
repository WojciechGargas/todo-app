using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Todo.Application.Exceptions;
using Todo.Core.Exceptions;

namespace Todo.Infrastructure.Exceptions;

internal sealed class ExceptionMiddleware(ILogger<ExceptionMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            logger.Log(LogLevel.Error, exception, exception.Message);
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, error) = exception switch
        {
            TaskAccessDeniedException => (StatusCodes.Status403Forbidden, new Error("forbidden", exception.Message)),
            CustomException => (StatusCodes.Status400BadRequest, new Error(exception
                .GetType().Name.Replace("Exception", string.Empty).Underscore(), exception.Message)),
            _ => (StatusCodes.Status500InternalServerError, new Error("error", "There was an error.")),
        };
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(error);
    }

    private record Error(string Code, string Reason);
}
