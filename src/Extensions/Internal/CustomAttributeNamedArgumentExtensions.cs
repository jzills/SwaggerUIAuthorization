using System.Reflection;
using SwaggerUIAuthorization.Constants;

namespace SwaggerUIAuthorization.Extensions.Internal;

/// <summary>
/// An <c>class</c> representing <c>CustomAttributeNamedArgument</c> extension methods.
/// </summary>
internal static class CustomAttributeNamedArgumentExtensions
{
    /// <summary>
    /// Determines if the attribute arguments indicate that only the authentication scheme is being verified.
    /// </summary>
    /// <param name="attributeArgs">The list of custom attribute arguments to check.</param>
    /// <returns><c>true</c> if there is only one argument and its name matches the authentication scheme constant; otherwise, <c>false</c>.</returns>
    internal static bool IsAuthenticationSchemeVerificationOnly(
        this IList<CustomAttributeNamedArgument> attributeArgs
    ) => attributeArgs.Count == 1 && 
        attributeArgs[0].MemberName == AuthenticationConstants.AuthenticationSchemes;

    /// <summary>
    /// Attempts to get authentication schemes in <c>IList&lt;CustomAttributeNamedArgument&gt;</c>.
    /// </summary>
    /// <param name="attributeArgs">An <c>IList&lt;CustomAttributeNamedArgument&gt;</c>.</param>
    /// <param name="authenticationSchemes">An <c>IEnumerable&lt;string&gt;</c> representing found authentication schemes.</param>
    /// <returns>A <c>bool</c> indicating the success of the operation.</returns>
    internal static bool TryGetAuthenticationSchemes(
        this IList<CustomAttributeNamedArgument> attributeArgs, 
        out IEnumerable<string> authenticationSchemes
    )
    {
        authenticationSchemes = attributeArgs
            .Where(attributeArg => attributeArg.MemberName == AuthenticationConstants.AuthenticationSchemes)
            .Select(authenticationScheme => authenticationScheme.TypedValue.Value as string ?? string.Empty)
            .Where(authenticationScheme => !string.IsNullOrWhiteSpace(authenticationScheme));

        return authenticationSchemes?.Any() ?? false;
    }

    /// <summary>
    /// Attempts to get any roles in the collection of <c>CustomAttributeNamedArgument</c>.
    /// </summary>
    /// <param name="attributeArgs">An <c>IList&lt;CustomAttributeNamedArgument&gt;</c>.</param>
    /// <param name="roles">An <c>IEnumerable&lt;string&gt;</c> representing found role names.</param>
    /// <returns>A <c>bool</c> indicating the success of the operation.</returns>
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

    /// <summary>
    /// Attempts to get any policies in the collection of <c>CustomAttributeNamedArgument</c>.
    /// </summary>
    /// <param name="attributeArgs">An <c>IList&lt;CustomAttributeNamedArgument&gt;</c>.</param>
    /// <param name="policy">An <c>string</c> representing found policies.</param>
    /// <returns>A <c>bool</c> indicating the success of the operation.</returns>
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

    /// <summary>
    /// Converts an <c>IList&lt;CustomAttributeNamedArgument&gt;</c> to an <c>IDictionary&lt;string, string&gt;</c> where
    /// the key is the <c>MemberName</c> and the value is the <c>TypedValue</c> value.
    /// </summary>
    /// <param name="args">An <c>IList&lt;CustomAttributeNamedArgument&gt;</c>.</param>
    /// <returns>A <c>bool</c> indicating the success of the operation.</returns>
    internal static IDictionary<string, string> ToMemberValueDictionary(
        this IList<CustomAttributeNamedArgument> args
    ) => args.ToDictionary(
            element => element.MemberName, 
            element => element.TypedValue.Value as string ?? string.Empty
        );
}