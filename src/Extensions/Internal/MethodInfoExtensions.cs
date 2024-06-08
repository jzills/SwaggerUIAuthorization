using System.Reflection;
using Microsoft.AspNetCore.Authorization;

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
    /// Attemps to get an <c>AuthorizeAttribute</c> from a specified <c>MethodInfo</c>.
    /// </summary>
    /// <param name="methodInfo">An instance of <c>MethodInfo</c>.</param>
    /// <param name="attributes">An <c>IEnumerable&lt;CustomAttributeData&gt;</c>.</param>
    /// <returns>A <c>bool</c> indicating the success of the operation.</returns>
    internal static bool TryGetAuthorizationAttribute(
        this MethodInfo methodInfo, 
        out IEnumerable<CustomAttributeData> attributes
    )
    {
        attributes = GetCustomAttributes<AuthorizeAttribute>(methodInfo);
        return attributes.Any();
    }

    /// <summary>
    /// Attemps to get an <c>AllowAnonymousAttribute</c> from a specified <c>MethodInfo</c>.
    /// </summary>
    /// <param name="methodInfo">An instance of <c>MethodInfo</c>.</param>
    /// <param name="attributes">An <c>IEnumerable&lt;CustomAttributeData&gt;</c>.</param>
    /// <returns>A <c>bool</c> indicating the success of the operation.</returns>
    internal static bool TryGetAllowAnonymousAttribute(
        this MethodInfo methodInfo, 
        out IEnumerable<CustomAttributeData> attributes
    )
    {
        attributes = GetCustomAttributes<AllowAnonymousAttribute>(methodInfo);
        return attributes.Any();
    }
}