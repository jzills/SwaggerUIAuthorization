using SwaggerUIAuthorization.Enums;

namespace SwaggerUIAuthorization.Extensions.Internal;

/// <summary>
/// An <c>class</c> representing <c>IDictionary</c> extension methods.
/// </summary>
internal static class IDictionaryExtensions
{
    /// <summary>
    /// Attemps to get an <c>AuthorizationTypes.Roles</c> key value from a source.
    /// </summary>
    /// <param name="source">A source of <c>IDictionary&lt;string, string&gt;</c>.</param>
    /// <param name="roleValue">A <c>string</c> representing the value of a role.</param>
    /// <returns>A <c>bool</c> indicating the success of the operation.</returns>
    internal static bool TryGetRoleValue(
        this IDictionary<string, string> source, 
        out string roleValue
    ) => source.TryGetValueInternal(nameof(AuthorizationTypes.Roles), out roleValue);

    /// <summary>
    /// Attemps to get an <c>AuthorizationTypes.Policy</c> key value from a source.
    /// </summary>
    /// <param name="source">A source of <c>IDictionary&lt;string, string&gt;</c>.</param>
    /// <param name="policyValue">A <c>string</c> representing the value of a policy.</param>
    /// <returns>A <c>bool</c> indicating the success of the operation.</returns>
    internal static bool TryGetPolicyValue(
        this IDictionary<string, string> source, 
        out string policyValue
    ) => source.TryGetValueInternal(nameof(AuthorizationTypes.Policy), out policyValue);

    /// <summary>
    /// Attemps to get a non null, empty or whitespace <c>string</c> key value from a source.
    /// </summary>
    /// <param name="source">A source of <c>IDictionary&lt;string, string&gt;</c>.</param>
    /// <param name="key">A <c>string</c> representing the key.</param>
    /// <param name="value">A <c>string</c> representing the value.</param>
    /// <returns>A <c>bool</c> indicating the success of the operation.</returns>
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