namespace SwaggerUIAuthorization.Components;

internal class SwaggerAuthenticationOptions
{
    public SwaggerAuthenticationOptions() {}
    
    public SwaggerAuthenticationOptions(
        string authenticationScheme
    ) => AuthenticationScheme = authenticationScheme;

    public string? RoutePrefix { get; set; } = "/swagger";
    public string? AuthenticationScheme { get; set; }
}