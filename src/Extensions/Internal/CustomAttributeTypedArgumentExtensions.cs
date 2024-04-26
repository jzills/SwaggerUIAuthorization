using System.Reflection;

namespace SwaggerUIAuthorization.Extensions.Internal;

internal static class CustomAttributeArgumentExtensions
{
    internal static bool TryGetPolicy(
        this IList<CustomAttributeTypedArgument> attributeArgs, 
        out string policy
    )
    {
        // Handle AuthorizeAttribute string constructor for policy
        var policyArg = attributeArgs.FirstOrDefault();
        if (policyArg.Value is not null && 
            policyArg.ArgumentType == typeof(string))
        {
            policy = (string)policyArg.Value!;
            return true;
        }
        else
        {
            policy = default!;
            return false;
        }
    }
}