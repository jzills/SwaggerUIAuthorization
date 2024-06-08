using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using SwaggerUIAuthorization.Components;
using SwaggerUIAuthorization.Extensions.Internal;

namespace SwaggerUIAuthorization.Filters;

/// <summary>
/// A <c>class</c> representing a <c>SwaggerAccessDocumentFilter</c>.
/// </summary>
internal class SwaggerAccessDocumentFilter : IDocumentFilter
{
    /// <summary>
    /// An <c>ISwaggerOperationCollection</c> containing Swagger operations.
    /// </summary>
    private readonly ISwaggerOperationCollection _operations;

    /// <summary>
    /// Creates an instance of <c>SwaggerAccessDocumentFilter</c>.
    /// </summary>
    /// <param name="operations">An <c>ISwaggerOperationCollection</c> containing Swagger operations.</param>
    /// <returns>An instance of <c>SwaggerAccessDocumentFilter</c>.</returns>
    public SwaggerAccessDocumentFilter(
        ISwaggerOperationCollection operations
    ) => _operations = operations;

    /// <summary>
    /// Validates a set <c>OpenApiDocument</c> operations based on their membership
    /// to this <c>ISwaggerOperationCollection</c>.
    /// </summary>
    /// <param name="swaggerDoc">An <c>OpenApiDocument</c>.</param>
    /// <param name="context">A <c>DocumentFilterContext</c>.</param>
    public void Apply(
        OpenApiDocument swaggerDoc, 
        DocumentFilterContext context
    )
    {
        foreach (var pathValue in swaggerDoc.Paths.Values)
        {
            foreach (var operation in pathValue.Operations)
            {
                var actionId = operation.Value.Tags.GetActionId();
                if (!_operations.IsAllowed(actionId))
                {
                    pathValue.Operations.Remove(operation);
                }
            }
        }
    }
}