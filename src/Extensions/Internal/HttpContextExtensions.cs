using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace SwaggerUIAuthorization.Extensions.Internal;

/// <summary>
/// An <c>class</c> representing <c>HttpContext</c> extension methods.
/// </summary>
internal static class HttpContextExtensions
{
    /// <summary>
    /// Attemps to get a <c>ClaimsPrincipal</c> from the current <c>HtpContext</c>.
    /// </summary>
    /// <param name="context">A <c>HttpContext</c>.</param>
    /// <param name="user">A <c>ClaimsPrincipal</c>.</param>
    /// <returns>A <c>bool</c> indicating the success of the operation.</returns>
    internal static bool TryGetAuthenticatedUser(
        this HttpContext context, 
        out ClaimsPrincipal user
    )
    {
        user = context.User;
        return context.User?.Identity?.IsAuthenticated ?? false;
    }

    /// <summary>
    /// Determines if the request path for the current <c>HttpContext</c> is to a Swagger document.
    /// </summary>
    /// <param name="context">A <c>HttpContext</c>.</param>
    /// <param name="routePrefix">A <c>string</c> representing the Swagger route prefix.</param>
    /// <returns>A <c>bool</c> indicating the if the route path matches the specified prefix.</returns>
    internal static bool IsRoutingToSwagger(this HttpContext context, string routePrefix) =>
        context.Request.Path.HasValue && 
        context.Request.Path.Value.StartsWith(
            routePrefix, 
            StringComparison.InvariantCultureIgnoreCase
        );
}