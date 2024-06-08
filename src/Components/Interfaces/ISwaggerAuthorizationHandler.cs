using System.Reflection;

namespace SwaggerUIAuthorization.Components;

/// <summary>
/// An interface representing an <c>ISwaggerAuthorizationHandler</c>.
/// </summary>
internal interface ISwaggerAuthorizationHandler
{
    /// <summary>
    /// Determines if a Swagger endpoint will be rendered in the document.
    /// </summary>
    /// <param name="attributeData">An instance of <c>CustomAttributeData</c>.</param>
    /// <returns>A <c>bool</c> representing whether or not a document element will be rendered.</returns>
    bool ShouldRender(CustomAttributeData attributeData);
}