using System.Reflection;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using NUnit.Framework;
using SwaggerUIAuthorization.Extensions.Internal;

namespace SwaggerUIAuthorization.Extensions.Internal.Tests;

[TestFixture]
public class MethodInfoExtensionsTests
{
    [Authorize]
    private static class DecoratedClass
    {
        public static void MethodOnDecoratedClass() { }
    }

    private static class UndecoratedClass
    {
        [Authorize("Policy")]
        public static void MethodWithAttribute() { }

        public static void MethodWithoutAttribute() { }

        [Authorize("PolicyA")]
        [Authorize("PolicyB")]
        public static void MethodWithMultipleAttributes() { }
    }

    [Test]
    public void TryGetCustomAttribute_WhenMethodHasAttribute_ReturnsTrue()
    {
        var methodInfo = typeof(UndecoratedClass).GetMethod(nameof(UndecoratedClass.MethodWithAttribute))!;

        var result = methodInfo.TryGetCustomAttribute<AuthorizeAttribute>(out var attributes);

        result.Should().BeTrue();
        attributes.Should().ContainSingle();
    }

    [Test]
    public void TryGetCustomAttribute_WhenMethodDoesNotHaveAttribute_ReturnsFalse()
    {
        var methodInfo = typeof(UndecoratedClass).GetMethod(nameof(UndecoratedClass.MethodWithoutAttribute))!;

        var result = methodInfo.TryGetCustomAttribute<AuthorizeAttribute>(out var attributes);

        result.Should().BeFalse();
        attributes.Should().BeEmpty();
    }

    [Test]
    public void TryGetCustomAttribute_WhenAttributeIsOnDeclaringType_ReturnsTrue()
    {
        var methodInfo = typeof(DecoratedClass).GetMethod(nameof(DecoratedClass.MethodOnDecoratedClass))!;

        var result = methodInfo.TryGetCustomAttribute<AuthorizeAttribute>(out var attributes);

        result.Should().BeTrue();
        attributes.Should().ContainSingle();
    }

    [Test]
    public void TryGetCustomAttribute_WhenMethodHasMultipleAttributes_ReturnsAllOfThem()
    {
        var methodInfo = typeof(UndecoratedClass).GetMethod(nameof(UndecoratedClass.MethodWithMultipleAttributes))!;

        var result = methodInfo.TryGetCustomAttribute<AuthorizeAttribute>(out var attributes);

        result.Should().BeTrue();
        attributes.Should().HaveCount(2);
    }

    [Test]
    public void GetCustomAttributes_WhenMethodHasAttribute_ReturnsAttribute()
    {
        var methodInfo = typeof(UndecoratedClass).GetMethod(nameof(UndecoratedClass.MethodWithAttribute))!;

        var attributes = MethodInfoExtensions.GetCustomAttributes<AuthorizeAttribute>(methodInfo);

        attributes.Should().ContainSingle();
    }

    [Test]
    public void GetCustomAttributes_WhenAttributeIsOnDeclaringType_ReturnsAttribute()
    {
        var methodInfo = typeof(DecoratedClass).GetMethod(nameof(DecoratedClass.MethodOnDecoratedClass))!;

        var attributes = MethodInfoExtensions.GetCustomAttributes<AuthorizeAttribute>(methodInfo);

        attributes.Should().ContainSingle();
    }
}
