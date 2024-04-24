namespace SwaggerUIAuthorization.Components;

internal interface ISwaggerAuthorizationProvider
{
    bool IsAuthorized(IEnumerable<string> roles);
    bool IsAuthorized(string? policy);
}