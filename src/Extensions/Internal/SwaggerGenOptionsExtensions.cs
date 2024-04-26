using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using SwaggerUIAuthorization.Filters;

namespace SwaggerUIAuthorization.Extensions.Internal;

internal static class SwaggerGenOptionsExtensions
{
    internal static void AddSwaggerAuthorizationFilters(this SwaggerGenOptions options)
    {
        options.DocumentFilter<SwaggerAccessDocumentFilter>();
        options.OperationFilter<SwaggerAccessOperationFilter>();
    }
}