using System.Reflection;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using NUnit.Framework;
using SwaggerUIAuthorization.Extensions.Internal;

namespace SwaggerUIAuthorization.Extensions.Internal.Tests;

[TestFixture]
public class CustomAttributeTypedArgumentExtensionsTests
{
    private static IList<CustomAttributeTypedArgument> GetConstructorArgs(string methodName) =>
        typeof(AttributedMethods)
            .GetMethod(methodName, BindingFlags.Public | BindingFlags.Static)!
            .GetCustomAttributesData()
            .First(attribute => attribute.AttributeType == typeof(AuthorizeAttribute))
            .ConstructorArguments;

    private static class AttributedMethods
    {
        [Authorize("AdminPolicy")]
        public static void WithPolicyConstructorArg() { }

        [Authorize]
        public static void WithNoArgs() { }
    }

    [Test]
    public void TryGetPolicy_WhenFirstArgumentIsString_ReturnsTrueAndPolicy()
    {
        var args = GetConstructorArgs(nameof(AttributedMethods.WithPolicyConstructorArg));

        var result = args.TryGetPolicy(out var policy);

        result.Should().BeTrue();
        policy.Should().Be("AdminPolicy");
    }

    [Test]
    public void TryGetPolicy_WhenNoConstructorArguments_ReturnsFalse()
    {
        var args = new List<CustomAttributeTypedArgument>();

        var result = args.TryGetPolicy(out var policy);

        result.Should().BeFalse();
        policy.Should().BeNull();
    }

    [Test]
    public void TryGetPolicy_WhenFirstArgumentIsNotString_ReturnsFalse()
    {
        // Construct a typed argument whose value is not a string
        var intArg = new CustomAttributeTypedArgument(typeof(int), 42);
        var args = new List<CustomAttributeTypedArgument> { intArg };

        var result = args.TryGetPolicy(out var policy);

        result.Should().BeFalse();
        policy.Should().BeNull();
    }
}
