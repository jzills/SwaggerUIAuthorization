using System.Reflection;

namespace SwaggerUIAuthorization.Extensions.Internal;

/// <summary>
/// An <c>class</c> representing <c>CustomAttributeArgument</c> extension methods.
/// </summary>
internal static class CustomAttributeArgumentExtensions
{
    /// <summary>
    /// Attempts to get a policy from <c>IList&lt;CustomAttributeTypedArgument&gt;</c>
    /// </summary>
    /// <param name="attributeArgs">An <c>IList&lt;CustomAttributeTypedArgument&gt;</c>.</param>
    /// <param name="policy">A <c>string</c> representing a found policy.</param>
    /// <returns>A <c>bool</c> indicating the success of the operation.</returns>
    internal static bool TryGetPolicy(
        this IList<CustomAttributeTypedArgument> attributeArgs, 
        out string policy
    )
    {
        // Handle AuthorizeAttribute string constructor for policy
        var policyArg = attributeArgs.FirstOrDefault();
        if (policyArg.Value is string policyValue)
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
}