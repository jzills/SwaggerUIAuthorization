using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using SwaggerUIAuthorization.Extensions.Internal;

namespace SwaggerUIAuthorization.Components;

/// <summary>
/// A <c>class</c> representing <c>SwaggerAuthenticationMiddleware</c> middleware.
/// </summary>
internal class SwaggerAuthenticationMiddleware
{
    /// <summary>
    /// A <c>RequestDelete</c> to continue the middleware pipeline.
    /// </summary>
    private readonly RequestDelegate _next;

    /// <summary>
    /// A <c>SwaggerAuthenticationOptions</c> used to determine the Swagger route prefix
    /// and required authentication scheme.
    /// </summary>
    private readonly SwaggerAuthenticationOptions _options;

    /// <summary>
    /// Creates an instance of <c>SwaggerAuthenticationMiddleware</c> middleware.
    /// </summary>
    /// <param name="next">A <c>RequestDelete</c> to continue the middleware pipeline.</param>
    /// <param name="options">A <c>SwaggerAuthenticationOptions</c> used to determine the Swagger route prefix
    /// and required authentication scheme.</param>
    /// <returns>An instance of <c>SwaggerAuthenticationMiddleware</c>.</returns>
    public SwaggerAuthenticationMiddleware(
        RequestDelegate next, 
        SwaggerAuthenticationOptions options
    ) => (_next, _options) = (next, options);

    /// <summary>
    /// Handles Swagger route checking and authentication.
    /// </summary>
    /// <param name="context">A <c>HttpContext</c>.</param>
    /// <returns>A <c>Task</c>.</returns>
    public Task InvokeAsync(HttpContext context)
    {
        if (context.IsRoutingToSwagger($"/{_options.RoutePrefix}"))
        {
            var isAuthenticated = context.User?.Identity?.IsAuthenticated ?? false; 
            if (!isAuthenticated)
            {
                return context.ChallengeAsync(_options.AuthenticationScheme);
            }
        }

        return _next(context);
    }
}