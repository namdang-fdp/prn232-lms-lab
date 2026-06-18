using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PRN232.LMS.API.Swagger;

public class AuthorizeCheckOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var hasAuthorize = context.MethodInfo.DeclaringType?.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() == true
            || context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();
        var hasAllowAnonymous = context.MethodInfo.DeclaringType?.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any() == true
            || context.MethodInfo.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any();

        if (!hasAuthorize || hasAllowAnonymous)
        {
            return;
        }

        operation.Responses.TryAdd(StatusCodes.Status401Unauthorized.ToString(), new OpenApiResponse { Description = "Unauthorized" });
        operation.Responses.TryAdd(StatusCodes.Status403Forbidden.ToString(), new OpenApiResponse { Description = "Forbidden" });
        operation.Security =
        [
            new OpenApiSecurityRequirement
            {
                [
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    }
                ] = []
            }
        ];
    }
}
