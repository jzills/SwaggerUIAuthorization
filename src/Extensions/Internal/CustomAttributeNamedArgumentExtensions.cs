using System.Reflection;

namespace SwaggerUIAuthorization.Extensions.Internal;

internal static class CustomAttributeNamedArgumentExtensions
{
    internal static bool TryGetAuthenticationSchemes(
        this IList<CustomAttributeNamedArgument> attributeArgs, 
        out IEnumerable<string> authenticationSchemes
    )
    {
        authenticationSchemes = attributeArgs
            .Where(arg => arg.MemberName == "AuthenticationSchemes")
            .Select(authenticationScheme => authenticationScheme.TypedValue.Value as string ?? string.Empty)
            .Where(authenticationScheme => !string.IsNullOrWhiteSpace(authenticationScheme));

        return authenticationSchemes?.Any() ?? false;
    }

    internal static bool TryGetRoles(
        this IList<CustomAttributeNamedArgument> attributeArgs, 
        out IEnumerable<string> roles
    )
    {
        var args = attributeArgs.ToMemberValueDictionary();
        if (args?.TryGetRoleValue(out var roleValue) ?? false)
        {
            roles = roleValue.Split(",").Select(value => value.Trim());
            return true;
        }
        else
        {
            roles = Enumerable.Empty<string>();
            return false;
        }
    }

    internal static bool TryGetPolicy(
        this IList<CustomAttributeNamedArgument> attributeArgs, 
        out string policy
    )
    {
        var args = attributeArgs.ToMemberValueDictionary();
        if (args?.TryGetPolicyValue(out var policyValue) ?? false)
        {
            policy = policyValue;
            return true;
        }
        else
        {
            policy = default!;
            return false;
        }
    }

    internal static IDictionary<string, string> ToMemberValueDictionary(
        this IList<CustomAttributeNamedArgument> args
    ) => args.ToDictionary(
            element => element.MemberName, 
            element => element.TypedValue.Value as string ?? string.Empty
        );
}