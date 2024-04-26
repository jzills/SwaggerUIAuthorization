using Microsoft.OpenApi.Models;

namespace SwaggerUIAuthorization.Extensions.Internal;

internal static class OpenApiTagExtensions
{
    internal static readonly string ActionTagName = $"ActionTag_{Guid.NewGuid()}";

    internal static void Add(this IList<OpenApiTag> tags, string actionId) =>
        tags.Add(new OpenApiTag
        {
            Name = ActionTagName,
            Description = actionId
        });

    internal static string GetActionId(this IList<OpenApiTag> tags)
    {
        var tag = tags.First(tag => tag.Name == ActionTagName);
        tags.Remove(tag);
        return tag.Description;
    }
}