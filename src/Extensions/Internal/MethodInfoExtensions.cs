using System.Reflection;
using Microsoft.AspNetCore.Authorization;

namespace SwaggerUIAuthorization.Extensions;

internal static class MethodInfoExtensions
{
    internal static IEnumerable<CustomAttributeData> GetCustomAttributes<TAttribute>(MethodInfo methodInfo) where TAttribute : Attribute
    {
        return methodInfo.CustomAttributes
            .Union(methodInfo.DeclaringType?.CustomAttributes ?? Enumerable.Empty<CustomAttributeData>())
            .Where(attribute => attribute.AttributeType == typeof(TAttribute));
    }

    public static bool TryGetAuthorizationAttribute(
        this MethodInfo methodInfo, 
        out IEnumerable<CustomAttributeData> attributes
    )
    {
        attributes = GetCustomAttributes<AuthorizeAttribute>(methodInfo);
        return attributes.Any();
    }

    public static bool TryGetAllowAnonymousAttribute(
        this MethodInfo methodInfo, 
        out IEnumerable<CustomAttributeData> attributes
    )
    {
        attributes = GetCustomAttributes<AllowAnonymousAttribute>(methodInfo);
        return attributes.Any();
    }
}