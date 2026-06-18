using System.Net;
using System.Text.Json;
using PRN232.LMS.API.Common.Exceptions;
using PRN232.LMS.API.Common.Response;
using PRN232.LMS.Services.Exceptions;

namespace PRN232.LMS.API.Common.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            if (context.Response.HasStarted)
            {
                _logger.LogError(exception, "An exception occurred after the response started.");
                throw;
            }

            await HandleExceptionAsync(context, exception);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.Clear();
        context.Response.ContentType = "application/json";

        var response = new ApiResponse<object>
        {
            Success = false,
            Data = null,
            Errors = null
        };

        switch (exception)
        {
            case ApiException apiEx:
                _logger.LogWarning(apiEx, "API exception handled with status code {StatusCode}.", apiEx.StatusCode);
                context.Response.StatusCode = apiEx.StatusCode;
                response.Message = apiEx.Message;
                break;
            case ServiceException serviceEx:
                _logger.LogWarning(serviceEx, "Service exception handled with status code {StatusCode}.", serviceEx.StatusCode);
                context.Response.StatusCode = serviceEx.StatusCode;
                response.Message = serviceEx.Message;
                response.Errors = serviceEx.Errors;
                break;
            default:
                _logger.LogError(exception, "Unhandled exception occurred while processing the request.");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Message = "Internal server error.";
                break;
        }

        var result = JsonSerializer.Serialize(
            response,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        return context.Response.WriteAsync(result);
    }
}
