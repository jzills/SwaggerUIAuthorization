using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using SwaggerUIAuthorization.Extensions;

namespace SwaggerUIAuthorization.Components;

internal class SwaggerAuthorizationHandler : ISwaggerAuthorizationHandler
{
    protected readonly IHttpContextAccessor HttpContextAccessor;
    protected readonly ISwaggerAuthorizationProvider AuthorizationProvider;

    public SwaggerAuthorizationHandler(
        IHttpContextAccessor httpContextAccessor,
        ISwaggerAuthorizationProvider authorizationProvider
    ) => (HttpContextAccessor, AuthorizationProvider) = (httpContextAccessor, authorizationProvider);

    public bool ShouldRender(CustomAttributeData attributeData) =>
        attributeData switch
        {
            { ConstructorArguments: { Count: > 0 } } => ShouldRender(attributeData.ConstructorArguments),
            { NamedArguments: { Count: > 0 } }       => ShouldRender(attributeData.NamedArguments),
            _                                        => false
        };

    private bool ShouldRender(IList<CustomAttributeNamedArgument> args)
    {
        if (args.TryGetAuthenticationSchemes(out var authenticationSchemes))
        {
            var isAuthenticated = authenticationSchemes.Any(authenticationScheme => 
                AuthenticateSynchronously(authenticationScheme));

            if (!isAuthenticated)
            {
                return false;
            }

            var isAuthenticationSchemeVerificationOnly = 
                args.Count == 1 && 
                args[0].MemberName == "AuthenticationSchemes";

            if (isAuthenticationSchemeVerificationOnly)
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

    private bool ShouldRender(IList<CustomAttributeTypedArgument> args)
    {
        if (args.TryGetPolicy(out var policy))
        {
            return AuthorizationProvider.IsAuthorized(policy);
        }
        else
        {
            return false;
        }
    }

    private bool AuthenticateSynchronously(string authenticationScheme) =>
        HttpContextAccessor.HttpContext
            ?.AuthenticateAsync(authenticationScheme)
            ?.Result?.Succeeded ?? false;
}