using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using SwaggerUIAuthorization.Components;
using SwaggerUIAuthorization.Extensions;

namespace SwaggerUIAuthorization.Filters;

internal class SwaggerAccessDocumentFilter : IDocumentFilter
{
    // private readonly ISwaggerSchemaCollection _schemas;
    private readonly ISwaggerOperationCollection _operations;

    public SwaggerAccessDocumentFilter(
        // ISwaggerSchemaCollection schemas,
        ISwaggerOperationCollection operations
    )
    {
        // _schemas = schemas;
        _operations = operations;
    }

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

        // TODO:    Figure out how to conditionally remove schema but
        //          for now, the fix is to not allow schema in UseSwaggerUI configuration.
        // TODO:    Cannot arbitrarily remove schemas without tags, i.e. DateOnly, etc
        //          because they will be missing from the larger schema, i.e. WeatherForecast
        // _schemas.RemoveIfNotRequired(context.SchemaRepository.Schemas);
    }
}