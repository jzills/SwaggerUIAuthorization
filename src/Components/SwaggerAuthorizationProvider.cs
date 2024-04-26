using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using SwaggerUIAuthorization.Extensions.Internal;

namespace SwaggerUIAuthorization.Components;

internal class SwaggerAuthorizationProvider : ISwaggerAuthorizationProvider
{
    protected readonly IHttpContextAccessor HttpContextAccessor;
    protected readonly IAuthorizationService AuthorizationService;

    public SwaggerAuthorizationProvider(
        IHttpContextAccessor httpContextAccessor,
        IAuthorizationService authorizationService
    )
    {
        HttpContextAccessor = httpContextAccessor;
        AuthorizationService = authorizationService;
    }

    public bool IsAuthorized(IEnumerable<string>? roles) =>
        roles?.Any(role => 
            HttpContextAccessor?.HttpContext?.User.IsInRole(role) ?? false) 
                ?? false;

    public bool IsAuthorized(string? policy)
    {
        if (HttpContextAccessor?.HttpContext?.TryGetAuthenticatedUser(out var user) ?? false)
        {
            return AuthorizationService
                .AuthorizeAsync(user, resource: null, policy!)
                    .Result.Succeeded;
        }
        else
        {
            return false;
        }
    }
}