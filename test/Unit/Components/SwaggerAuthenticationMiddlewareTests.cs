using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using System.Security.Claims;
using SwaggerUIAuthorization.Components;

namespace SwaggerUIAuthorization.Components.Tests;

[TestFixture]
public class SwaggerAuthenticationMiddlewareTests
{
    private Mock<RequestDelegate> _nextMock;
    private SwaggerAuthenticationOptions _options;
    private SwaggerAuthenticationMiddleware _sut;

    [SetUp]
    public void SetUp()
    {
        _nextMock = new Mock<RequestDelegate>();
        _nextMock.Setup(next => next(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);
        _options = new SwaggerAuthenticationOptions
        {
            RoutePrefix = "swagger",
            AuthenticationScheme = "Cookies"
        };
        _sut = new SwaggerAuthenticationMiddleware(_nextMock.Object, _options);
    }

    [Test]
    public async Task InvokeAsync_WhenNotSwaggerRoute_CallsNext()
    {
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/values";

        await _sut.InvokeAsync(context);

        _nextMock.Verify(next => next(context), Times.Once);
    }

    [Test]
    public async Task InvokeAsync_WhenSwaggerRouteAndAuthenticated_CallsNext()
    {
        var context = new DefaultHttpContext();
        context.Request.Path = "/swagger/index.html";
        context.User = new ClaimsPrincipal(new ClaimsIdentity("test"));

        await _sut.InvokeAsync(context);

        _nextMock.Verify(next => next(context), Times.Once);
    }

    [Test]
    public async Task InvokeAsync_WhenSwaggerRouteAndNotAuthenticated_CallsChallengeNotNext()
    {
        var authServiceMock = new Mock<IAuthenticationService>();
        authServiceMock
            .Setup(service => service.ChallengeAsync(It.IsAny<HttpContext>(), "Cookies", null))
            .Returns(Task.CompletedTask);

        var context = new DefaultHttpContext();
        context.Request.Path = "/swagger/index.html";
        context.User = new ClaimsPrincipal(new ClaimsIdentity());
        context.RequestServices = new ServiceCollection()
            .AddSingleton(authServiceMock.Object)
            .BuildServiceProvider();

        await _sut.InvokeAsync(context);

        authServiceMock.Verify(
            service => service.ChallengeAsync(It.IsAny<HttpContext>(), "Cookies", null),
            Times.Once);
        _nextMock.Verify(next => next(context), Times.Never);
    }

    [Test]
    public async Task InvokeAsync_WhenSwaggerJsonRouteAndNotAuthenticated_CallsChallenge()
    {
        var authServiceMock = new Mock<IAuthenticationService>();
        authServiceMock
            .Setup(service => service.ChallengeAsync(It.IsAny<HttpContext>(), "Cookies", null))
            .Returns(Task.CompletedTask);

        var context = new DefaultHttpContext();
        context.Request.Path = "/swagger/v1/swagger.json";
        context.User = new ClaimsPrincipal(new ClaimsIdentity());
        context.RequestServices = new ServiceCollection()
            .AddSingleton(authServiceMock.Object)
            .BuildServiceProvider();

        await _sut.InvokeAsync(context);

        authServiceMock.Verify(
            service => service.ChallengeAsync(It.IsAny<HttpContext>(), "Cookies", null),
            Times.Once);
    }

    [Test]
    public async Task InvokeAsync_WhenPathContainsSwaggerButDoesNotStartWithPrefix_CallsNext()
    {
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/swagger-docs";

        await _sut.InvokeAsync(context);

        _nextMock.Verify(next => next(context), Times.Once);
    }

    [Test]
    public async Task InvokeAsync_WhenPathMatchesPrefixCaseInsensitive_AndNotAuthenticated_CallsChallenge()
    {
        var authServiceMock = new Mock<IAuthenticationService>();
        authServiceMock
            .Setup(service => service.ChallengeAsync(It.IsAny<HttpContext>(), "Cookies", null))
            .Returns(Task.CompletedTask);

        var context = new DefaultHttpContext();
        context.Request.Path = "/SWAGGER/index.html";
        context.User = new ClaimsPrincipal(new ClaimsIdentity());
        context.RequestServices = new ServiceCollection()
            .AddSingleton(authServiceMock.Object)
            .BuildServiceProvider();

        await _sut.InvokeAsync(context);

        authServiceMock.Verify(
            service => service.ChallengeAsync(It.IsAny<HttpContext>(), "Cookies", null),
            Times.Once);
    }
}
