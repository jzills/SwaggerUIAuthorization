using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using SwaggerUIAuthorization.Filters;

namespace SwaggerUIAuthorization.Extensions.Internal;

/// <summary>
/// An <c>class</c> representing <c>SwaggerGenOptions</c> extension methods.
/// </summary>
internal static class SwaggerGenOptionsExtensions
{
    /// <summary>
    /// Adds required authorization filters to the DI container.
    /// </summary>
    /// <param name="options">An instance of <c>SwaggerGenOptions</c>.</param>
    internal static void AddSwaggerAuthorizationFilters(this SwaggerGenOptions options)
    {
        options.DocumentFilter<SwaggerAccessDocumentFilter>();
        options.OperationFilter<SwaggerAccessOperationFilter>();
    }
}