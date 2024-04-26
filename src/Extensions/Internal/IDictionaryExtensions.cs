using SwaggerUIAuthorization.Enums;

namespace SwaggerUIAuthorization.Extensions.Internal;

internal static class IDictionaryExtensions
{
    internal static bool TryGetRoleValue(
        this IDictionary<string, string> source, 
        out string roleValue
    ) => source.TryGetValueInternal(nameof(AuthorizationTypes.Roles), out roleValue);

    internal static bool TryGetPolicyValue(
        this IDictionary<string, string> source, 
        out string policyValue
    ) => source.TryGetValueInternal(nameof(AuthorizationTypes.Policy), out policyValue);

    private static bool TryGetValueInternal(
        this IDictionary<string, string> source, 
        string key,
        out string value
    )
    {
        if ( source.TryGetValue(key, out string? possibleValue) &&
            !string.IsNullOrWhiteSpace(possibleValue))
        {
            value = possibleValue;
        }
        else
        {
            value = default!;
        }

        return !string.IsNullOrWhiteSpace(value);
    }
}