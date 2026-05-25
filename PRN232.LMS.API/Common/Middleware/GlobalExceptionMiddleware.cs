using System.Net;
using System.Text.Json;
using PRN232.LMS.API.Common.Response;
using PRN232.LMS.API.Common.Exceptions;
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ApiResponse<object>
        {
            Success = false,
            Data = null
        };

        switch (exception)
        {
            case ApiException apiEx:
                context.Response.StatusCode = apiEx.StatusCode;
                response.Message = apiEx.Message;
                break;
            case ServiceException serviceEx:
                context.Response.StatusCode = serviceEx.StatusCode;
                response.Message = serviceEx.Message;
                response.Errors = serviceEx.Errors;
                break;
            // Catch FluentValidation errors later here
            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Message = "Internal Server Error";
                // Optionally add stack trace in dev env, but keep it secure for prod
                break;
        }

        var result = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        return context.Response.WriteAsync(result);
    }
}
