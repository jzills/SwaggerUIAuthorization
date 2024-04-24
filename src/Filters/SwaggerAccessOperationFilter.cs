using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using SwaggerUIAuthorization.Components;
using SwaggerUIAuthorization.Extensions;

namespace SwaggerUIAuthorization.Filters;

internal class SwaggerAccessOperationFilter : IOperationFilter
{
    private readonly ISwaggerOperationCollection _operations;
    private readonly ISwaggerAuthorizationHandler _authorizationHandler;

    public SwaggerAccessOperationFilter(
        ISwaggerOperationCollection operations,
        ISwaggerAuthorizationHandler authorizationHandler
    ) 
    {
        _operations = operations;
        _authorizationHandler = authorizationHandler;
    }

    public void Apply(
        OpenApiOperation operation, 
        OperationFilterContext context
    )
    {
        var actionId = context.ApiDescription.ActionDescriptor.Id;
        operation.Tags.Add(actionId);

        if ( context.MethodInfo.TryGetAuthorizationAttribute(out var attributes) &&
            !context.MethodInfo.TryGetAllowAnonymousAttribute(out var _))
        {
            var shouldRender = attributes.All(attribute => _authorizationHandler.ShouldRender(attribute));
            if (shouldRender)
            {
                _operations.Add(actionId);
            }
        }
        else
        {
            _operations.Add(actionId);
        }
    }
}