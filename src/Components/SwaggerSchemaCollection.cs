// using Microsoft.OpenApi.Models;

// namespace SwaggerUIAuthorization.Components;

// internal class SwaggerSchemaCollection : ISwaggerSchemaCollection
// {
//     private readonly HashSet<string> _tagNames = new();

//     public void AddRange(IList<OpenApiTag> tags)
//     {
//         foreach (var tag in tags)
//         {
//             _tagNames.Add(tag.Name);
//         }
//     }

//     public void RemoveIfNotRequired(IDictionary<string, OpenApiSchema> schemas)
//     {
//         foreach (var key in schemas.Keys)
//         {
//             var isReferenced = _tagNames.Any(tag => schemas[tag].Properties.Any(property => property.Value.Reference?.Id == key));
//             if (!_tagNames.Contains(key) && !isReferenced)
//             {
//                 schemas.Remove(key);
//             }
//         }
//     }
// }