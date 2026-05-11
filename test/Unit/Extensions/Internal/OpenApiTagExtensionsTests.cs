using FluentAssertions;
using Microsoft.OpenApi.Models;
using NUnit.Framework;
using SwaggerUIAuthorization.Extensions.Internal;

namespace SwaggerUIAuthorization.Extensions.Internal.Tests;

[TestFixture]
public class OpenApiTagExtensionsTests
{
    [Test]
    public void Add_WhenValidActionId_AddsTagWithActionTagNameAndDescription()
    {
        var tags = new List<OpenApiTag>();

        tags.Add("my-action-id");

        tags.Should().ContainSingle();
        tags[0].Name.Should().Be(OpenApiTagExtensions.ActionTagName);
        tags[0].Description.Should().Be("my-action-id");
    }

    [Test]
    public void Add_WhenCalledMultipleTimes_AddsMultipleTags()
    {
        var tags = new List<OpenApiTag>();

        tags.Add("action-1");
        tags.Add("action-2");

        tags.Should().HaveCount(2);
        tags.Should().OnlyContain(tag => tag.Name == OpenApiTagExtensions.ActionTagName);
    }

    [Test]
    public void GetActionId_WhenTagExists_ReturnsDescriptionAndRemovesTag()
    {
        var tags = new List<OpenApiTag>();
        tags.Add("test-action");

        var actionId = tags.GetActionId();

        actionId.Should().Be("test-action");
        tags.Should().BeEmpty();
    }

    [Test]
    public void GetActionId_WhenMultipleTagsExist_ReturnsFirstActionTagAndRemovesIt()
    {
        var tags = new List<OpenApiTag>
        {
            new OpenApiTag { Name = "SomeOtherTag", Description = "other" }
        };
        tags.Add("target-action");

        var actionId = tags.GetActionId();

        actionId.Should().Be("target-action");
        tags.Should().ContainSingle(tag => tag.Name == "SomeOtherTag");
    }

    [Test]
    public void GetActionId_WhenNoActionTagExists_ThrowsInvalidOperationException()
    {
        var tags = new List<OpenApiTag>
        {
            new OpenApiTag { Name = "UnrelatedTag", Description = "value" }
        };

        Action act = () => tags.GetActionId();

        act.Should().Throw<InvalidOperationException>();
    }
}
