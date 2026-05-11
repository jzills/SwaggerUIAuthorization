using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Moq;
using NUnit.Framework;
using Swashbuckle.AspNetCore.SwaggerGen;
using SwaggerUIAuthorization.Components;
using SwaggerUIAuthorization.Extensions.Internal;
using SwaggerUIAuthorization.Filters;

namespace SwaggerUIAuthorization.Filters.Tests;

[TestFixture]
public class SwaggerAccessDocumentFilterTests
{
    private Mock<ISwaggerOperationCollection> _operationsMock;
    private SwaggerAccessDocumentFilter _sut;

    [SetUp]
    public void SetUp()
    {
        _operationsMock = new Mock<ISwaggerOperationCollection>();
        _sut = new SwaggerAccessDocumentFilter(_operationsMock.Object);
    }

    private static DocumentFilterContext CreateContext()
    {
        var schemaGeneratorMock = new Mock<ISchemaGenerator>();
        return new DocumentFilterContext(
            Enumerable.Empty<ApiDescription>(),
            schemaGeneratorMock.Object,
            new SchemaRepository());
    }

    [Test]
    public void Apply_WhenDocumentHasNoPaths_DoesNotThrow()
    {
        var doc = new OpenApiDocument { Paths = new OpenApiPaths() };

        Action act = () => _sut.Apply(doc, CreateContext());

        act.Should().NotThrow();
        _operationsMock.Verify(ops => ops.HasOperation(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void Apply_WhenOperationIsInCollection_KeepsOperation()
    {
        const string actionId = "test-action-1";

        var operation = new OpenApiOperation { Tags = new List<OpenApiTag>() };
        operation.Tags.Add(actionId);

        var pathItem = new OpenApiPathItem();
        pathItem.Operations[OperationType.Get] = operation;

        var doc = new OpenApiDocument
        {
            Paths = new OpenApiPaths { ["/api/test"] = pathItem }
        };

        _operationsMock.Setup(ops => ops.HasOperation(actionId)).Returns(true);

        _sut.Apply(doc, CreateContext());

        doc.Paths["/api/test"].Operations.Should().ContainKey(OperationType.Get);
    }

    [Test]
    public void Apply_WhenOperationIsInCollection_InvokesHasOperation()
    {
        const string actionId = "test-action-2";

        var operation = new OpenApiOperation { Tags = new List<OpenApiTag>() };
        operation.Tags.Add(actionId);

        var pathItem = new OpenApiPathItem();
        pathItem.Operations[OperationType.Post] = operation;

        var doc = new OpenApiDocument
        {
            Paths = new OpenApiPaths { ["/api/items"] = pathItem }
        };

        _operationsMock.Setup(ops => ops.HasOperation(actionId)).Returns(true);

        _sut.Apply(doc, CreateContext());

        _operationsMock.Verify(ops => ops.HasOperation(actionId), Times.Once);
    }

    [Test]
    public void Apply_WhenMultiplePathsAndAllOperationsInCollection_KeepsAllPaths()
    {
        const string actionId1 = "action-get";
        const string actionId2 = "action-post";

        var getOperation = new OpenApiOperation { Tags = new List<OpenApiTag>() };
        getOperation.Tags.Add(actionId1);
        var getPathItem = new OpenApiPathItem();
        getPathItem.Operations[OperationType.Get] = getOperation;

        var postOperation = new OpenApiOperation { Tags = new List<OpenApiTag>() };
        postOperation.Tags.Add(actionId2);
        var postPathItem = new OpenApiPathItem();
        postPathItem.Operations[OperationType.Post] = postOperation;

        var doc = new OpenApiDocument
        {
            Paths = new OpenApiPaths
            {
                ["/api/items"] = getPathItem,
                ["/api/create"] = postPathItem
            }
        };

        _operationsMock.Setup(ops => ops.HasOperation(actionId1)).Returns(true);
        _operationsMock.Setup(ops => ops.HasOperation(actionId2)).Returns(true);

        _sut.Apply(doc, CreateContext());

        doc.Paths["/api/items"].Operations.Should().ContainKey(OperationType.Get);
        doc.Paths["/api/create"].Operations.Should().ContainKey(OperationType.Post);
    }
}
