using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using SwaggerUIAuthorization.Components;
using SwaggerUIAuthorization.Extensions.Internal;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SwaggerUIAuthorization.Filters;

/// <summary>
/// A <c>class</c> representing a <c>SwaggerAccessOperationFilter</c>.
/// </summary>
internal class SwaggerAccessOperationFilter : IOperationFilter
{
    /// <summary>
    /// An <c>ISwaggerOperationCollection</c> containing Swagger operations.
    /// </summary>
    private readonly ISwaggerOperationCollection _operations;

    /// <summary>
    /// An <c>ISwaggerAuthorizationHandler</c> used to authorize Swagger endpoint visibility.
    /// </summary>
    private readonly ISwaggerAuthorizationHandler _authorizationHandler;

    /// <summary>
    /// Creates an instance of <c>SwaggerAccessOperationFilter</c>.
    /// </summary>
    /// <param name="operations">An <c>ISwaggerOperationCollection</c> containing Swagger operations.</param>
    /// <param name="authorizationHandler">An <c>ISwaggerAuthorizationHandler</c> used to authorize Swagger endpoint visibility.</param>
    /// <returns>An instance of <c>SwaggerAccessOperationFilter</c>.</returns>
    public SwaggerAccessOperationFilter(
        ISwaggerOperationCollection operations,
        ISwaggerAuthorizationHandler authorizationHandler
    ) 
    {
        _operations = operations;
        _authorizationHandler = authorizationHandler;
    }

    /// <summary>
    /// Authenticates the specified operation against endpoint <c>AuthorizeAttribute</c> values.
    /// </summary>
    /// <remarks>
    /// If authentication is successful, the operation is added 
    /// to this <c>ISwaggerOperationCollection</c>. Any operations that do not pass 
    /// authentication are then removed in the <c>SwaggerAccessDocumentFilter</c>.
    /// </remarks>
    /// <param name="operation">An <c>OpenApiOperation</c>.</param>
    /// <param name="context">An <c>OperationFilterContext</c>.</param>
    public void Apply(
        OpenApiOperation operation, 
        OperationFilterContext context
    )
    {
        var actionId = context.ApiDescription.ActionDescriptor.Id;
        operation.Tags.Add(actionId);

        // First check for AllowAnonymousAttribute since
        // the presence of one will override any AuthorizeAttribute from running.
        if (context.MethodInfo.TryGetCustomAttribute<AllowAnonymousAttribute>(out var _))
        {
            _operations.Add(actionId);
        }
        else if (context.MethodInfo.TryGetCustomAttribute<AuthorizeAttribute>(out var attributes))
        {
            // Multiple AuthorizeAttribute's are joined with AND expressions.
            var shouldRender = attributes.All(_authorizationHandler.ShouldRender);
            if (shouldRender)
            {
                _operations.Add(actionId);
            }
        }
        else
        {
            // Nothing to see here...
            _operations.Add(actionId);
        }
    }
}