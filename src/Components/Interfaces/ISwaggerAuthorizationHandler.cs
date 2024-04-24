using System.Reflection;

namespace SwaggerUIAuthorization.Components;

internal interface ISwaggerAuthorizationHandler
{
    bool ShouldRender(CustomAttributeData attributeData);
}