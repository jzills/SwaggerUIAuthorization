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
        ArgumentNullException.ThrowIfNull(httpContextAccessor.HttpContext, nameof(httpContextAccessor.HttpContext));
        ArgumentNullException.ThrowIfNull(httpContextAccessor.HttpContext.User, nameof(httpContextAccessor.HttpContext.User));

        HttpContextAccessor = httpContextAccessor;
        AuthorizationService = authorizationService;
    }

    /// <inheritdoc/>
    public bool IsAuthorized(IEnumerable<string>? roles) =>
        roles?.Any(HttpContextAccessor!.HttpContext!.User.IsInRole) ?? false;

    /// <inheritdoc/>
    public bool IsAuthorized(string? policy) =>
        HttpContextAccessor!.HttpContext!.TryGetAuthenticatedUser(out var user) &&
            AuthorizationService
                .AuthorizeAsync(user, resource: null, policy!)
                    .Result.Succeeded;
}