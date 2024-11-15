using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using SwaggerUIAuthorization.Extensions.Internal;
using SwaggerUIAuthorization.Constants;

namespace SwaggerUIAuthorization.Components;

/// <summary>
/// A <c>class</c> representing <c>SwaggerAuthorizationHandler</c>.
/// </summary>
internal class SwaggerAuthorizationHandler : ISwaggerAuthorizationHandler
{
    /// <summary>
    /// An <c>IHttpContextAccessor</c> used to authenticate requests.
    /// </summary>
    protected readonly IHttpContextAccessor HttpContextAccessor;

    /// <summary>
    /// An <c>ISwaggerAuthorizationProvider</c> used to authorize Swagger endpoints.
    /// </summary>
    protected readonly ISwaggerAuthorizationProvider AuthorizationProvider;

    /// <summary>
    /// Creates an instance of <c>SwaggerAuthorizationHandler</c>.
    /// </summary>
    /// <param name="httpContextAccessor">An <c>IHttpContextAccessor</c>.</param>
    /// <param name="authorizationProvider">An <c>ISwaggerAuthorizationProvider</c>.</param>
    /// <returns>An instance of <c>SwaggerAuthorizationHandler</c>.</returns>
    public SwaggerAuthorizationHandler(
        IHttpContextAccessor httpContextAccessor,
        ISwaggerAuthorizationProvider authorizationProvider
    ) => (HttpContextAccessor, AuthorizationProvider) = (httpContextAccessor, authorizationProvider);

    /// <inheritdoc/>
    public bool ShouldRender(CustomAttributeData attributeData) =>
        attributeData switch
        {
            { ConstructorArguments: { Count: > 0 } } => ShouldRender(attributeData.ConstructorArguments),
            { NamedArguments: { Count: > 0 } }       => ShouldRender(attributeData.NamedArguments),
            _                                        => false
        };

    /// <summary>
    /// Determines if a Swagger endpoint will be rendered in the document.
    /// </summary>
    /// <param name="args">A collection of <c>CustomAttributeNamedArgument</c>.</param>
    /// <returns>A <c>bool</c> representing whether or not a document element will be rendered.</returns>
    private bool ShouldRender(IList<CustomAttributeNamedArgument> args)
    {
        if (args.TryGetAuthenticationSchemes(out var authenticationSchemes))
        {
            var isAuthenticated = authenticationSchemes.Any(AuthenticateSynchronously);
            if (!isAuthenticated)
            {
                return false;
            }

            if (args.IsAuthenticationSchemeVerificationOnly())
            {
                return true;
            }
        }

        if (args.TryGetRoles(out var roles))
        {
            return AuthorizationProvider.IsAuthorized(roles);
        }
        else if (args.TryGetPolicy(out var policy))
        {
            return AuthorizationProvider.IsAuthorized(policy);
        }
        else
        {
            return false;
        }
    } 

    /// <summary>
    /// Determines if a Swagger endpoint will be rendered in the document.
    /// </summary>
    /// <param name="args">A collection of <c>CustomAttributeTypedArgument</c>.</param>
    /// <returns>A <c>bool</c> representing whether or not a document element will be rendered.</returns>
    private bool ShouldRender(IList<CustomAttributeTypedArgument> args) =>
        args.TryGetPolicy(out var policy) && 
            AuthorizationProvider.IsAuthorized(policy);

    /// <summary>
    /// Authenticates the specified scheme synchronously.
    /// </summary>
    /// <param name="authenticationScheme">A <c>string</c> representing the name of the authentication scheme.</param>
    /// <returns>A <c>bool</c> representing the success of authentication.</returns>
    private bool AuthenticateSynchronously(string authenticationScheme) =>
        HttpContextAccessor.HttpContext
            ?.AuthenticateAsync(authenticationScheme)
            ?.Result?.Succeeded ?? false;
}