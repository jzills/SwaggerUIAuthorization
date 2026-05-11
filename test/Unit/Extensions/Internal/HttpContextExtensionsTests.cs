using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using System.Security.Claims;
using SwaggerUIAuthorization.Extensions.Internal;

namespace SwaggerUIAuthorization.Extensions.Internal.Tests;

[TestFixture]
public class HttpContextExtensionsTests
{
    [Test]
    public void TryGetAuthenticatedUser_WhenUserIsAuthenticated_ReturnsTrueAndSetsUser()
    {
        var identity = new ClaimsIdentity("test");
        var expectedUser = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext { User = expectedUser };

        var result = context.TryGetAuthenticatedUser(out var user);

        result.Should().BeTrue();
        user.Should().BeSameAs(expectedUser);
    }

    [Test]
    public void TryGetAuthenticatedUser_WhenUserIsNotAuthenticated_ReturnsFalseAndSetsUser()
    {
        var unauthenticatedUser = new ClaimsPrincipal(new ClaimsIdentity());
        var context = new DefaultHttpContext { User = unauthenticatedUser };

        var result = context.TryGetAuthenticatedUser(out var user);

        result.Should().BeFalse();
        user.Should().BeSameAs(unauthenticatedUser);
    }

    [Test]
    public void IsRoutingToSwagger_WhenPathMatchesPrefix_ReturnsTrue()
    {
        var context = new DefaultHttpContext();
        context.Request.Path = "/swagger/index.html";

        context.IsRoutingToSwagger("/swagger").Should().BeTrue();
    }

    [Test]
    public void IsRoutingToSwagger_WhenPathExactlyMatchesPrefix_ReturnsTrue()
    {
        var context = new DefaultHttpContext();
        context.Request.Path = "/swagger";

        context.IsRoutingToSwagger("/swagger").Should().BeTrue();
    }

    [Test]
    public void IsRoutingToSwagger_WhenPathDoesNotMatchPrefix_ReturnsFalse()
    {
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/values";

        context.IsRoutingToSwagger("/swagger").Should().BeFalse();
    }

    [Test]
    public void IsRoutingToSwagger_WhenPathMatchesCaseInsensitive_ReturnsTrue()
    {
        var context = new DefaultHttpContext();
        context.Request.Path = "/SWAGGER/v1/swagger.json";

        context.IsRoutingToSwagger("/swagger").Should().BeTrue();
    }

    [Test]
    public void IsRoutingToSwagger_WhenPathHasNoValue_ReturnsFalse()
    {
        var context = new DefaultHttpContext();
        // Path.HasValue is false when Path is PathString.Empty

        context.IsRoutingToSwagger("/swagger").Should().BeFalse();
    }

    [Test]
    public void IsRoutingToSwagger_WhenPathContainsPrefixButDoesNotStartWithIt_ReturnsFalse()
    {
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/swagger-docs";

        context.IsRoutingToSwagger("/swagger").Should().BeFalse();
    }
}
