namespace SwaggerUIAuthorization.Components;

/// <summary>
/// An interface representing an <c>ISwaggerOperationCollection</c>.
/// </summary>
internal interface ISwaggerOperationCollection
{
    /// <summary>
    /// Adds an operation identifier to this <c>ISwaggerOperationCollection</c>.
    /// </summary>
    /// <param name="operationId">A <c>string</c> representing an operation.</param>
    void Add(string operationId);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="operationId">A <c>string</c> representing an operation.</param>
    /// <returns>A <c>bool</c> representing whether or not a document element will be rendered.</returns>
    bool IsAllowed(string operationId);
}