namespace SwaggerUIAuthorization.Components;

internal class SwaggerOperationCollection : ISwaggerOperationCollection
{
    private readonly HashSet<string> _operations = new();
    
    public void Add(string operationId)
    {
        if (!string.IsNullOrWhiteSpace(operationId))
        {
            _operations.Add(operationId);
        }
    }

    public bool IsAllowed(string operationId)
    {
        if (!string.IsNullOrWhiteSpace(operationId))
        {
            return _operations.Contains(operationId);
        }
        else
        {
            return false;
        }
    }
}