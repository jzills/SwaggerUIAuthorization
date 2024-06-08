using Microsoft.OpenApi.Models;

namespace SwaggerUIAuthorization.Extensions.Internal;

/// <summary>
/// An <c>class</c> representing <c>OpenApiTag</c> extension methods.
/// </summary>
internal static class OpenApiTagExtensions
{
    internal static readonly string ActionTagName = $"ActionTag_{Guid.NewGuid()}";

    /// <summary>
    /// Adds an <c>OpenApiTag</c> with the specified action.
    /// </summary>
    /// <param name="tags">An <c>IList&lt;OpenApiTag&gt;</c>.</param>
    /// <param name="actionId">A <c>string</c> representing an action identifier.</param>
    internal static void Add(this IList<OpenApiTag> tags, string actionId) =>
        tags.Add(new OpenApiTag
        {
            Name = ActionTagName,
            Description = actionId
        });

    /// <summary>
    /// Retrieves and removes the first <c>OpenApiTag</c> matched by <c>ActionTagName</c>.
    /// </summary>
    /// <param name="tags">An <c>IList&lt;OpenApiTag&gt;</c>.</param>
    /// <returns>A <c>string</c> representing an action identifier.</returns>
    internal static string GetActionId(this IList<OpenApiTag> tags)
    {
        var tag = tags.First(tag => tag.Name == ActionTagName);
        tags.Remove(tag);
        return tag.Description;
    }
}