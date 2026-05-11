using FluentAssertions;
using NUnit.Framework;
using SwaggerUIAuthorization.Components;

namespace SwaggerUIAuthorization.Components.Tests;

[TestFixture]
public class SwaggerOperationCollectionTests
{
    private SwaggerOperationCollection _sut;

    [SetUp]
    public void SetUp()
    {
        _sut = new SwaggerOperationCollection();
    }

    [Test]
    public void Add_WhenValidOperationId_AddsOperation()
    {
        _sut.Add("op-1");

        _sut.HasOperation("op-1").Should().BeTrue();
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void Add_WhenNullOrWhitespaceOperationId_DoesNotAdd(string? operationId)
    {
        _sut.Add(operationId!);

        _sut.HasOperation(operationId!).Should().BeFalse();
    }

    [Test]
    public void Add_WhenDuplicateOperationId_DoesNotThrow()
    {
        _sut.Add("op-1");

        Action act = () => _sut.Add("op-1");

        act.Should().NotThrow();
        _sut.HasOperation("op-1").Should().BeTrue();
    }

    [Test]
    public void HasOperation_WhenOperationDoesNotExist_ReturnsFalse()
    {
        _sut.HasOperation("missing").Should().BeFalse();
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void HasOperation_WhenNullOrWhitespaceOperationId_ReturnsFalse(string? operationId)
    {
        _sut.HasOperation(operationId!).Should().BeFalse();
    }

    [Test]
    public void HasOperation_WhenOperationExists_ReturnsTrue()
    {
        _sut.Add("op-abc");

        _sut.HasOperation("op-abc").Should().BeTrue();
    }

    [Test]
    public void HasOperation_WhenMultipleOperationsAdded_ReturnsTrueOnlyForAdded()
    {
        _sut.Add("op-1");
        _sut.Add("op-2");

        _sut.HasOperation("op-1").Should().BeTrue();
        _sut.HasOperation("op-2").Should().BeTrue();
        _sut.HasOperation("op-3").Should().BeFalse();
    }
}
