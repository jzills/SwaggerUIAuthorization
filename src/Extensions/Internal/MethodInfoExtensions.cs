using System.Reflection;

namespace SwaggerUIAuthorization.Extensions.Internal;

/// <summary>
/// An <c>class</c> representing <c>MethodInfo</c> extension methods.
/// </summary>
internal static class MethodInfoExtensions
{
    /// <summary>
    /// Retrieves <c>CustomAttributeData</c> of a specified <c>TAttribute</c> type.
    /// </summary>
    /// <param name="methodInfo">An instance of <c>MethodInfo</c>.</param>
    /// <typeparam name="TAttribute">The generic type parameter to match a type of <c>CustomAttributeData</c> against.</typeparam>
    /// <returns>An <c>IEnumerable&lt;CustomAttributeData&gt;</c>.</returns>
    internal static IEnumerable<CustomAttributeData> 
        GetCustomAttributes<TAttribute>(MethodInfo methodInfo) 
            where TAttribute : Attribute =>
                methodInfo.CustomAttributes
                    .Union(methodInfo.DeclaringType?.CustomAttributes ?? Enumerable.Empty<CustomAttributeData>())
                    .Where(attribute => attribute.AttributeType == typeof(TAttribute));

    /// <summary>
    /// Attemps to get a <c>TAttribute</c> from a specified <c>MethodInfo</c>.
    /// </summary>
    /// <param name="methodInfo">An instance of <c>MethodInfo</c>.</param>
    /// <param name="attributes">An <c>IEnumerable&lt;CustomAttributeData&gt;</c>.</param>
    /// <typeparam name="TAttribute">An <c>Attribute</c>.</typeparam> 
    /// <returns>A <c>bool</c> indicating the success of the operation.</returns>
    internal static bool TryGetCustomAttribute<TAttribute>(
        this MethodInfo methodInfo, 
        out IEnumerable<CustomAttributeData> attributes
    ) where TAttribute : Attribute
    {
        attributes = GetCustomAttributes<TAttribute>(methodInfo);
        return attributes.Any();
    }
}