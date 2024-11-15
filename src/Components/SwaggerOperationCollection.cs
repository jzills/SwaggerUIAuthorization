namespace SwaggerUIAuthorization.Components;

/// <summary>
/// A <c>class</c> representing <c>SwaggerOperationCollection</c>.
/// </summary>
internal class SwaggerOperationCollection : ISwaggerOperationCollection
{
    /// <summary>
    /// A <c>HashSet&lt;string&gt;</c> containing allowed operations.
    /// </summary>
    private readonly HashSet<string> _operations = new();
    
    /// <inheritdoc/>
    public void Add(string operationId)
    {
        if (!string.IsNullOrWhiteSpace(operationId))
        {
            _operations.Add(operationId);
        }
    }

    /// <inheritdoc/>
    public bool HasOperation(string operationId) =>
        !string.IsNullOrWhiteSpace(operationId) && 
            _operations.Contains(operationId);
}