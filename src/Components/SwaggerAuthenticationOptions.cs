namespace SwaggerUIAuthorization.Components;

/// <summary>
/// A <c>class</c> representing <c>SwaggerAuthenticationOptions</c>.
/// </summary>
internal class SwaggerAuthenticationOptions
{
    /// <summary>
    /// Creates an instance of <c>SwaggerAuthenticationOptions</c>.
    /// </summary>
    public SwaggerAuthenticationOptions() {}
    
    /// <summary>
    /// Creates an instance of <c>SwaggerAuthenticationOptions</c> with the specified authentication scheme.
    /// </summary>
    /// <param name="authenticationScheme">A <c>string</c> representing the authentication scheme to valdiate.</param>
    public SwaggerAuthenticationOptions(
        string authenticationScheme
    ) => AuthenticationScheme = authenticationScheme;

    /// <summary>
    /// The Swagger route prefix used to redirect to authentication.
    /// </summary>
    /// <value>A <c>string</c> representing the route prefix for the Swagger document.</value>
    public string? RoutePrefix { get; set; } = "/swagger";

    /// <summary>
    /// The authentication scheme used to authenticate Swagger document requests.
    /// </summary>
    /// <value>A <c>string</c> representing an authentication scheme.</value>
    public string? AuthenticationScheme { get; set; }
}