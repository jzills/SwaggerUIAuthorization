namespace SwaggerUIAuthorization.Components;

/// <summary>
/// An interface representing an <c>ISwaggerAuthorizationProvider</c>.
/// </summary>
internal interface ISwaggerAuthorizationProvider
{
    /// <summary>
    /// Checks if any role is authorized for access to a particular endpoint.
    /// </summary>
    /// <param name="roles">An <c>IEnumerable&lt;string&gt;</c> containing role names.</param>
    /// <returns>A <c>bool</c> representing whether or not a document element will be rendered.</returns>
    bool IsAuthorized(IEnumerable<string> roles);

    /// <summary>
    /// Checks if a policy is authorized for access to a particular endpoint.
    /// </summary>
    /// <param name="policy">An <c>string</c> representing a policy name.</param>
    /// <returns>A <c>bool</c> representing whether or not a document element will be rendered.</returns>
    bool IsAuthorized(string? policy);
}