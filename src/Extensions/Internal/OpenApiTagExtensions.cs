using Microsoft.OpenApi.Models;

namespace SwaggerUIAuthorization.Extensions;

internal static class OpenApiTagExtensions
{
    public static readonly string ActionTagName = $"ActionTag_{Guid.NewGuid()}";

    public static void Add(this IList<OpenApiTag> tags, string actionId) =>
        tags.Add(new OpenApiTag
        {
            Name = ActionTagName,
            Description = actionId
        });

    public static string GetActionId(this IList<OpenApiTag> tags)
    {
        var tag = tags.First(tag => tag.Name == ActionTagName);
        tags.Remove(tag);
        return tag.Description;
    }
}