using System.Reflection;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using System.Security.Claims;
using SwaggerUIAuthorization.Components;

namespace SwaggerUIAuthorization.Components.Tests;

[TestFixture]
public class SwaggerAuthorizationHandlerTests
{
    private Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private Mock<ISwaggerAuthorizationProvider> _authorizationProviderMock;
    private SwaggerAuthorizationHandler _sut;

    [SetUp]
    public void SetUp()
    {
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _authorizationProviderMock = new Mock<ISwaggerAuthorizationProvider>();
        _sut = new SwaggerAuthorizationHandler(_httpContextAccessorMock.Object, _authorizationProviderMock.Object);
    }

    private static CustomAttributeData GetAuthorizeAttributeData(string methodName) =>
        typeof(AttributedMethods)
            .GetMethod(methodName, BindingFlags.Public | BindingFlags.Static)!
            .GetCustomAttributesData()
            .First(attribute => attribute.AttributeType == typeof(AuthorizeAttribute));

    private static DefaultHttpContext CreateHttpContextWithAuthService(Mock<IAuthenticationService> authServiceMock)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.RequestServices = new ServiceCollection()
            .AddSingleton(authServiceMock.Object)
            .BuildServiceProvider();
        return httpContext;
    }

    // Helper methods with specific AuthorizeAttribute configurations for reflection-based tests
    private static class AttributedMethods
    {
        [Authorize("AdminPolicy")]
        public static void WithPolicyConstructorArg() { }

        [Authorize(Roles = "Admin")]
        public static void WithRoles() { }

        [Authorize(Policy = "AdminPolicy")]
        public static void WithNamedPolicy() { }

        [Authorize(AuthenticationSchemes = "Bearer")]
        public static void WithSchemeOnly() { }

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        public static void WithSchemeAndRoles() { }

        [Authorize]
        public static void WithNoArgs() { }
    }

    [Test]
    public void ShouldRender_WhenConstructorArgPolicyAndAuthorized_ReturnsTrue()
    {
        _authorizationProviderMock.Setup(provider => provider.IsAuthorized("AdminPolicy")).Returns(true);
        var attributeData = GetAuthorizeAttributeData(nameof(AttributedMethods.WithPolicyConstructorArg));

        _sut.ShouldRender(attributeData).Should().BeTrue();
    }

    [Test]
    public void ShouldRender_WhenConstructorArgPolicyAndNotAuthorized_ReturnsFalse()
    {
        _authorizationProviderMock.Setup(provider => provider.IsAuthorized("AdminPolicy")).Returns(false);
        var attributeData = GetAuthorizeAttributeData(nameof(AttributedMethods.WithPolicyConstructorArg));

        _sut.ShouldRender(attributeData).Should().BeFalse();
    }

    [Test]
    public void ShouldRender_WhenNamedRolesAndAuthorized_ReturnsTrue()
    {
        _authorizationProviderMock
            .Setup(provider => provider.IsAuthorized(It.IsAny<IEnumerable<string>>()))
            .Returns(true);
        var attributeData = GetAuthorizeAttributeData(nameof(AttributedMethods.WithRoles));

        _sut.ShouldRender(attributeData).Should().BeTrue();
    }

    [Test]
    public void ShouldRender_WhenNamedRolesAndNotAuthorized_ReturnsFalse()
    {
        _authorizationProviderMock
            .Setup(provider => provider.IsAuthorized(It.IsAny<IEnumerable<string>>()))
            .Returns(false);
        var attributeData = GetAuthorizeAttributeData(nameof(AttributedMethods.WithRoles));

        _sut.ShouldRender(attributeData).Should().BeFalse();
    }

    [Test]
    public void ShouldRender_WhenNamedPolicyAndAuthorized_ReturnsTrue()
    {
        _authorizationProviderMock.Setup(provider => provider.IsAuthorized("AdminPolicy")).Returns(true);
        var attributeData = GetAuthorizeAttributeData(nameof(AttributedMethods.WithNamedPolicy));

        _sut.ShouldRender(attributeData).Should().BeTrue();
    }

    [Test]
    public void ShouldRender_WhenNamedPolicyAndNotAuthorized_ReturnsFalse()
    {
        _authorizationProviderMock.Setup(provider => provider.IsAuthorized("AdminPolicy")).Returns(false);
        var attributeData = GetAuthorizeAttributeData(nameof(AttributedMethods.WithNamedPolicy));

        _sut.ShouldRender(attributeData).Should().BeFalse();
    }

    [Test]
    public void ShouldRender_WhenNoArguments_ReturnsFalse()
    {
        var attributeData = GetAuthorizeAttributeData(nameof(AttributedMethods.WithNoArgs));

        _sut.ShouldRender(attributeData).Should().BeFalse();
    }

    [Test]
    public void ShouldRender_WhenSchemeOnlyAndAuthenticationSucceeds_ReturnsTrue()
    {
        var authServiceMock = new Mock<IAuthenticationService>();
        authServiceMock
            .Setup(service => service.AuthenticateAsync(It.IsAny<HttpContext>(), "Bearer"))
            .ReturnsAsync(AuthenticateResult.Success(
                new AuthenticationTicket(new ClaimsPrincipal(), "Bearer")));

        var httpContext = CreateHttpContextWithAuthService(authServiceMock);
        _httpContextAccessorMock.Setup(accessor => accessor.HttpContext).Returns(httpContext);
        var attributeData = GetAuthorizeAttributeData(nameof(AttributedMethods.WithSchemeOnly));

        _sut.ShouldRender(attributeData).Should().BeTrue();
    }

    [Test]
    public void ShouldRender_WhenSchemeOnlyAndAuthenticationFails_ReturnsFalse()
    {
        var authServiceMock = new Mock<IAuthenticationService>();
        authServiceMock
            .Setup(service => service.AuthenticateAsync(It.IsAny<HttpContext>(), "Bearer"))
            .ReturnsAsync(AuthenticateResult.Fail("auth failed"));

        var httpContext = CreateHttpContextWithAuthService(authServiceMock);
        _httpContextAccessorMock.Setup(accessor => accessor.HttpContext).Returns(httpContext);
        var attributeData = GetAuthorizeAttributeData(nameof(AttributedMethods.WithSchemeOnly));

        _sut.ShouldRender(attributeData).Should().BeFalse();
    }

    [Test]
    public void ShouldRender_WhenSchemeAndRolesAndAuthSucceedsAndRoleAuthorized_ReturnsTrue()
    {
        var authServiceMock = new Mock<IAuthenticationService>();
        authServiceMock
            .Setup(service => service.AuthenticateAsync(It.IsAny<HttpContext>(), "Bearer"))
            .ReturnsAsync(AuthenticateResult.Success(
                new AuthenticationTicket(new ClaimsPrincipal(), "Bearer")));

        var httpContext = CreateHttpContextWithAuthService(authServiceMock);
        _httpContextAccessorMock.Setup(accessor => accessor.HttpContext).Returns(httpContext);
        _authorizationProviderMock
            .Setup(provider => provider.IsAuthorized(It.IsAny<IEnumerable<string>>()))
            .Returns(true);
        var attributeData = GetAuthorizeAttributeData(nameof(AttributedMethods.WithSchemeAndRoles));

        _sut.ShouldRender(attributeData).Should().BeTrue();
    }

    [Test]
    public void ShouldRender_WhenSchemeAndRolesAndAuthenticationFails_ReturnsFalse()
    {
        var authServiceMock = new Mock<IAuthenticationService>();
        authServiceMock
            .Setup(service => service.AuthenticateAsync(It.IsAny<HttpContext>(), "Bearer"))
            .ReturnsAsync(AuthenticateResult.Fail("auth failed"));

        var httpContext = CreateHttpContextWithAuthService(authServiceMock);
        _httpContextAccessorMock.Setup(accessor => accessor.HttpContext).Returns(httpContext);
        var attributeData = GetAuthorizeAttributeData(nameof(AttributedMethods.WithSchemeAndRoles));

        _sut.ShouldRender(attributeData).Should().BeFalse();
    }

    [Test]
    public void ShouldRender_WhenHttpContextIsNull_ReturnsFalse()
    {
        _httpContextAccessorMock.Setup(accessor => accessor.HttpContext).Returns((HttpContext?)null);
        var attributeData = GetAuthorizeAttributeData(nameof(AttributedMethods.WithSchemeOnly));

        _sut.ShouldRender(attributeData).Should().BeFalse();
    }
}
