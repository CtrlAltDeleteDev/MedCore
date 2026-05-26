using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MedCore.Appointment.Api.Swagger;

/// <summary>
/// Adds a Bearer security requirement to every Swagger operation that has
/// [Authorize] (and is not [AllowAnonymous]). This makes Swagger UI include
/// the Authorization: Bearer header when the user has clicked Authorize.
/// </summary>
public sealed class BearerSecurityOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var hasAuthorize =
            context.MethodInfo.DeclaringType?
                .GetCustomAttributes(inherit: true).OfType<AuthorizeAttribute>().Any() is true
            || context.MethodInfo
                .GetCustomAttributes(inherit: true).OfType<AuthorizeAttribute>().Any();

        var hasAllowAnonymous =
            context.MethodInfo.DeclaringType?
                .GetCustomAttributes(inherit: true).OfType<AllowAnonymousAttribute>().Any() is true
            || context.MethodInfo
                .GetCustomAttributes(inherit: true).OfType<AllowAnonymousAttribute>().Any();

        if (!hasAuthorize || hasAllowAnonymous)
            return;

        operation.Security ??= [];
        operation.Security.Add(new OpenApiSecurityRequirement
        {
            { new OpenApiSecuritySchemeReference("Bearer"), [] }
        });
    }
}
