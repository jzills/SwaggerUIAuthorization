namespace SwaggerUIAuthorization.Components;

internal interface ISwaggerOperationCollection
{
    void Add(string operationId);
    bool IsAllowed(string operationId);
}