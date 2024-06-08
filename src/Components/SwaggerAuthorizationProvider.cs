using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using SwaggerUIAuthorization.Extensions.Internal;

namespace SwaggerUIAuthorization.Components;

/// <summary>
/// A <c>class</c> representing <c>SwaggerAuthorizationProvider</c>.
/// </summary>
internal class SwaggerAuthorizationProvider : ISwaggerAuthorizationProvider
{
    /// <summary>
    /// An <c>IHttpContextAccessor</c> used to authenticate requests.
    /// </summary>
    protected readonly IHttpContextAccessor HttpContextAccessor;

    /// <summary>
    /// An <c>IAuthorizationService</c> used to authorize a set of user roles or policies.
    /// </summary>
    protected readonly IAuthorizationService AuthorizationService;
    
    /// <summary>
    /// Creates an instance of <c>SwaggerAuthorizationProvider</c>.
    /// </summary>
    /// <param name="httpContextAccessor">An <c>IHttpContextAccessor</c>.</param>
    /// <param name="authorizationService">An <c>IAuthorizationService</c>.</param>
    /// <returns>An instance of <c>SwaggerAuthorizationProvider</c>.</returns>
    public SwaggerAuthorizationProvider(
        IHttpContextAccessor httpContextAccessor,
        IAuthorizationService authorizationService
    )
    {
        HttpContextAccessor = httpContextAccessor;
        AuthorizationService = authorizationService;
    }

    /// <inheritdoc/>
    public bool IsAuthorized(IEnumerable<string>? roles) =>
        roles?.Any(role => 
            HttpContextAccessor?.HttpContext?.User.IsInRole(role) ?? false) 
                ?? false;

    /// <inheritdoc/>
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