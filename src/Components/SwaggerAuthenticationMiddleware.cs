using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using SwaggerUIAuthorization.Extensions;

namespace SwaggerUIAuthorization.Components;

internal class SwaggerAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly SwaggerAuthenticationOptions _options;

    public SwaggerAuthenticationMiddleware(
        RequestDelegate next, 
        SwaggerAuthenticationOptions options
    ) => (_next, _options) = (next, options);

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