using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace SwaggerUIAuthorization.Extensions.Internal;

internal static class HttpContextExtensions
{
    internal static bool TryGetAuthenticatedUser(
        this HttpContext context, 
        out ClaimsPrincipal user
    )
    {
        user = context.User;
        return context.User?.Identity?.IsAuthenticated ?? false;
    }

    internal static bool IsRoutingToSwagger(this HttpContext context, string routePrefix) =>
        context.Request.Path.HasValue && 
        context.Request.Path.Value.StartsWith(
            routePrefix, 
            StringComparison.InvariantCultureIgnoreCase
        );
}