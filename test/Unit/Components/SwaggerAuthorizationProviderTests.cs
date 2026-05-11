using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using System.Security.Claims;
using SwaggerUIAuthorization.Components;

namespace SwaggerUIAuthorization.Components.Tests;

[TestFixture]
public class SwaggerAuthorizationProviderTests
{
    private Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private Mock<IAuthorizationService> _authorizationServiceMock;

    [SetUp]
    public void SetUp()
    {
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _authorizationServiceMock = new Mock<IAuthorizationService>();
    }

    private SwaggerAuthorizationProvider CreateSut(ClaimsPrincipal? user = null)
    {
        var httpContext = new DefaultHttpContext
        {
            User = user ?? new ClaimsPrincipal(new ClaimsIdentity("test"))
        };
        _httpContextAccessorMock.Setup(accessor => accessor.HttpContext).Returns(httpContext);
        return new SwaggerAuthorizationProvider(_httpContextAccessorMock.Object, _authorizationServiceMock.Object);
    }

    [Test]
    public void Constructor_WhenHttpContextIsNull_ThrowsArgumentNullException()
    {
        _httpContextAccessorMock.Setup(accessor => accessor.HttpContext).Returns((HttpContext?)null);

        Action act = () => new SwaggerAuthorizationProvider(_httpContextAccessorMock.Object, _authorizationServiceMock.Object);

        act.Should().Throw<ArgumentNullException>().WithParameterName("HttpContext");
    }

    [Test]
    public void Constructor_WhenUserIsNull_ThrowsArgumentNullException()
    {
        var httpContextMock = new Mock<HttpContext>();
        httpContextMock.Setup(ctx => ctx.User).Returns((ClaimsPrincipal)null!);
        _httpContextAccessorMock.Setup(accessor => accessor.HttpContext).Returns(httpContextMock.Object);

        Action act = () => new SwaggerAuthorizationProvider(_httpContextAccessorMock.Object, _authorizationServiceMock.Object);

        act.Should().Throw<ArgumentNullException>().WithParameterName("User");
    }

    [Test]
    public void IsAuthorized_Roles_WhenUserHasRole_ReturnsTrue()
    {
        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Role, "Admin") }, "test");
        var sut = CreateSut(new ClaimsPrincipal(identity));

        sut.IsAuthorized(new[] { "Admin" }).Should().BeTrue();
    }

    [Test]
    public void IsAuthorized_Roles_WhenUserDoesNotHaveRole_ReturnsFalse()
    {
        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Role, "User") }, "test");
        var sut = CreateSut(new ClaimsPrincipal(identity));

        sut.IsAuthorized(new[] { "Admin" }).Should().BeFalse();
    }

    [Test]
    public void IsAuthorized_Roles_WhenOneOfMultipleRolesMatches_ReturnsTrue()
    {
        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Role, "Admin") }, "test");
        var sut = CreateSut(new ClaimsPrincipal(identity));

        sut.IsAuthorized(new[] { "SuperAdmin", "Admin" }).Should().BeTrue();
    }

    [Test]
    public void IsAuthorized_Roles_WhenRolesIsNull_ReturnsFalse()
    {
        var sut = CreateSut();

        sut.IsAuthorized((IEnumerable<string>?)null).Should().BeFalse();
    }

    [Test]
    public void IsAuthorized_Roles_WhenRolesIsEmpty_ReturnsFalse()
    {
        var sut = CreateSut();

        sut.IsAuthorized(Enumerable.Empty<string>()).Should().BeFalse();
    }

    [Test]
    public void IsAuthorized_Policy_WhenUserIsAuthenticated_AndPolicySucceeds_ReturnsTrue()
    {
        var sut = CreateSut();
        _authorizationServiceMock
            .Setup(service => service.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), null, "AdminPolicy"))
            .ReturnsAsync(AuthorizationResult.Success());

        sut.IsAuthorized("AdminPolicy").Should().BeTrue();
    }

    [Test]
    public void IsAuthorized_Policy_WhenUserIsAuthenticated_AndPolicyFails_ReturnsFalse()
    {
        var sut = CreateSut();
        _authorizationServiceMock
            .Setup(service => service.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), null, "AdminPolicy"))
            .ReturnsAsync(AuthorizationResult.Failed());

        sut.IsAuthorized("AdminPolicy").Should().BeFalse();
    }

    [Test]
    public void IsAuthorized_Policy_WhenUserIsNotAuthenticated_ReturnsFalse()
    {
        var unauthenticatedUser = new ClaimsPrincipal(new ClaimsIdentity());
        var sut = CreateSut(unauthenticatedUser);

        sut.IsAuthorized("AdminPolicy").Should().BeFalse();

        _authorizationServiceMock.Verify(
            service => service.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()),
            Times.Never
        );
    }
}
