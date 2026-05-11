using System.Reflection;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.OpenApi.Models;
using Moq;
using NUnit.Framework;
using Swashbuckle.AspNetCore.SwaggerGen;
using SwaggerUIAuthorization.Components;
using SwaggerUIAuthorization.Extensions.Internal;
using SwaggerUIAuthorization.Filters;

namespace SwaggerUIAuthorization.Filters.Tests;

[TestFixture]
public class SwaggerAccessOperationFilterTests
{
    private Mock<ISwaggerOperationCollection> _operationsMock;
    private Mock<ISwaggerAuthorizationHandler> _authorizationHandlerMock;
    private SwaggerAccessOperationFilter _sut;

    [SetUp]
    public void SetUp()
    {
        _operationsMock = new Mock<ISwaggerOperationCollection>();
        _authorizationHandlerMock = new Mock<ISwaggerAuthorizationHandler>();
        _sut = new SwaggerAccessOperationFilter(_operationsMock.Object, _authorizationHandlerMock.Object);
    }

    private static OperationFilterContext CreateContext(MethodInfo methodInfo)
    {
        var actionDescriptor = new ActionDescriptor();
        var apiDescription = new ApiDescription { ActionDescriptor = actionDescriptor };
        var schemaGeneratorMock = new Mock<ISchemaGenerator>();
        return new OperationFilterContext(apiDescription, schemaGeneratorMock.Object, new SchemaRepository(), methodInfo);
    }

    // Helper class with methods decorated for specific filter paths
    private static class OperationHelpers
    {
        [AllowAnonymous]
        public static void AnonymousAction() { }

        [Authorize("AdminPolicy")]
        public static void AuthorizedAction() { }

        [Authorize(Roles = "Admin")]
        public static void RoleAction() { }

        [Authorize("PolicyA")]
        [Authorize("PolicyB")]
        public static void MultipleAuthorizeAction() { }

        public static void UndecoratedAction() { }
    }

    [Test]
    public void Apply_Always_AddsTagToOperation()
    {
        var methodInfo = typeof(OperationHelpers).GetMethod(nameof(OperationHelpers.UndecoratedAction))!;
        var context = CreateContext(methodInfo);
        var operation = new OpenApiOperation { Tags = new List<OpenApiTag>() };

        _sut.Apply(operation, context);

        operation.Tags.Should().ContainSingle(tag => tag.Name == OpenApiTagExtensions.ActionTagName);
    }

    [Test]
    public void Apply_WhenAllowAnonymousAttribute_AddsToOperationCollection()
    {
        var methodInfo = typeof(OperationHelpers).GetMethod(nameof(OperationHelpers.AnonymousAction))!;
        var context = CreateContext(methodInfo);
        var operation = new OpenApiOperation { Tags = new List<OpenApiTag>() };

        _sut.Apply(operation, context);

        _operationsMock.Verify(ops => ops.Add(context.ApiDescription.ActionDescriptor.Id), Times.Once);
    }

    [Test]
    public void Apply_WhenAllowAnonymousAttribute_DoesNotInvokeAuthorizationHandler()
    {
        var methodInfo = typeof(OperationHelpers).GetMethod(nameof(OperationHelpers.AnonymousAction))!;
        var context = CreateContext(methodInfo);
        var operation = new OpenApiOperation { Tags = new List<OpenApiTag>() };

        _sut.Apply(operation, context);

        _authorizationHandlerMock.Verify(
            handler => handler.ShouldRender(It.IsAny<System.Reflection.CustomAttributeData>()),
            Times.Never);
    }

    [Test]
    public void Apply_WhenAuthorizeAttributeAndAuthorized_AddsToOperationCollection()
    {
        _authorizationHandlerMock
            .Setup(handler => handler.ShouldRender(It.IsAny<System.Reflection.CustomAttributeData>()))
            .Returns(true);

        var methodInfo = typeof(OperationHelpers).GetMethod(nameof(OperationHelpers.AuthorizedAction))!;
        var context = CreateContext(methodInfo);
        var operation = new OpenApiOperation { Tags = new List<OpenApiTag>() };

        _sut.Apply(operation, context);

        _operationsMock.Verify(ops => ops.Add(context.ApiDescription.ActionDescriptor.Id), Times.Once);
    }

    [Test]
    public void Apply_WhenAuthorizeAttributeAndNotAuthorized_DoesNotAddToOperationCollection()
    {
        _authorizationHandlerMock
            .Setup(handler => handler.ShouldRender(It.IsAny<System.Reflection.CustomAttributeData>()))
            .Returns(false);

        var methodInfo = typeof(OperationHelpers).GetMethod(nameof(OperationHelpers.AuthorizedAction))!;
        var context = CreateContext(methodInfo);
        var operation = new OpenApiOperation { Tags = new List<OpenApiTag>() };

        _sut.Apply(operation, context);

        _operationsMock.Verify(ops => ops.Add(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void Apply_WhenNoAuthAttributes_AddsToOperationCollection()
    {
        var methodInfo = typeof(OperationHelpers).GetMethod(nameof(OperationHelpers.UndecoratedAction))!;
        var context = CreateContext(methodInfo);
        var operation = new OpenApiOperation { Tags = new List<OpenApiTag>() };

        _sut.Apply(operation, context);

        _operationsMock.Verify(ops => ops.Add(context.ApiDescription.ActionDescriptor.Id), Times.Once);
    }

    [Test]
    public void Apply_WhenMultipleAuthorizeAttributesAndAllPass_AddsToOperationCollection()
    {
        _authorizationHandlerMock
            .Setup(handler => handler.ShouldRender(It.IsAny<System.Reflection.CustomAttributeData>()))
            .Returns(true);

        var methodInfo = typeof(OperationHelpers).GetMethod(nameof(OperationHelpers.MultipleAuthorizeAction))!;
        var context = CreateContext(methodInfo);
        var operation = new OpenApiOperation { Tags = new List<OpenApiTag>() };

        _sut.Apply(operation, context);

        _operationsMock.Verify(ops => ops.Add(context.ApiDescription.ActionDescriptor.Id), Times.Once);
    }

    [Test]
    public void Apply_WhenMultipleAuthorizeAttributesAndOneFails_DoesNotAddToOperationCollection()
    {
        var callCount = 0;
        _authorizationHandlerMock
            .Setup(handler => handler.ShouldRender(It.IsAny<System.Reflection.CustomAttributeData>()))
            .Returns(() => callCount++ == 0); // first call true, second call false

        var methodInfo = typeof(OperationHelpers).GetMethod(nameof(OperationHelpers.MultipleAuthorizeAction))!;
        var context = CreateContext(methodInfo);
        var operation = new OpenApiOperation { Tags = new List<OpenApiTag>() };

        _sut.Apply(operation, context);

        _operationsMock.Verify(ops => ops.Add(It.IsAny<string>()), Times.Never);
    }
}
