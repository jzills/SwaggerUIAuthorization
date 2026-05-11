using System.Reflection;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using NUnit.Framework;
using SwaggerUIAuthorization.Extensions.Internal;

namespace SwaggerUIAuthorization.Extensions.Internal.Tests;

[TestFixture]
public class CustomAttributeNamedArgumentExtensionsTests
{
    private static IList<CustomAttributeNamedArgument> GetNamedArgs(string methodName) =>
        typeof(AttributedMethods)
            .GetMethod(methodName, BindingFlags.Public | BindingFlags.Static)!
            .GetCustomAttributesData()
            .First(attribute => attribute.AttributeType == typeof(AuthorizeAttribute))
            .NamedArguments;

    private static class AttributedMethods
    {
        [Authorize(Roles = "Admin")]
        public static void WithRoles() { }

        [Authorize(Roles = "Admin,SuperAdmin")]
        public static void WithMultipleRoles() { }

        [Authorize(Policy = "AdminPolicy")]
        public static void WithPolicy() { }

        [Authorize(AuthenticationSchemes = "Bearer")]
        public static void WithSchemeOnly() { }

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        public static void WithSchemeAndRoles() { }
    }

    [Test]
    public void TryGetRoles_WhenRolesNamedArgExists_ReturnsTrueAndRoles()
    {
        var args = GetNamedArgs(nameof(AttributedMethods.WithRoles));

        var result = args.TryGetRoles(out var roles);

        result.Should().BeTrue();
        roles.Should().ContainSingle().Which.Should().Be("Admin");
    }

    [Test]
    public void TryGetRoles_WhenCommaSeparatedRoles_ReturnsSplitAndTrimmedRoles()
    {
        var args = GetNamedArgs(nameof(AttributedMethods.WithMultipleRoles));

        var result = args.TryGetRoles(out var roles);

        result.Should().BeTrue();
        roles.Should().BeEquivalentTo(new[] { "Admin", "SuperAdmin" });
    }

    [Test]
    public void TryGetRoles_WhenNoRolesNamedArg_ReturnsFalse()
    {
        var args = GetNamedArgs(nameof(AttributedMethods.WithPolicy));

        var result = args.TryGetRoles(out var roles);

        result.Should().BeFalse();
        roles.Should().BeEmpty();
    }

    [Test]
    public void TryGetPolicy_WhenPolicyNamedArgExists_ReturnsTrueAndPolicy()
    {
        var args = GetNamedArgs(nameof(AttributedMethods.WithPolicy));

        var result = args.TryGetPolicy(out var policy);

        result.Should().BeTrue();
        policy.Should().Be("AdminPolicy");
    }

    [Test]
    public void TryGetPolicy_WhenNoPolicyNamedArg_ReturnsFalse()
    {
        var args = GetNamedArgs(nameof(AttributedMethods.WithRoles));

        var result = args.TryGetPolicy(out var policy);

        result.Should().BeFalse();
        policy.Should().BeNull();
    }

    [Test]
    public void TryGetAuthenticationSchemes_WhenSchemeNamedArgExists_ReturnsTrueAndSchemes()
    {
        var args = GetNamedArgs(nameof(AttributedMethods.WithSchemeOnly));

        var result = args.TryGetAuthenticationSchemes(out var schemes);

        result.Should().BeTrue();
        schemes.Should().ContainSingle().Which.Should().Be("Bearer");
    }

    [Test]
    public void TryGetAuthenticationSchemes_WhenNoSchemeNamedArg_ReturnsFalse()
    {
        var args = GetNamedArgs(nameof(AttributedMethods.WithRoles));

        var result = args.TryGetAuthenticationSchemes(out var schemes);

        result.Should().BeFalse();
    }

    [Test]
    public void IsAuthenticationSchemeVerificationOnly_WhenOnlySchemeArg_ReturnsTrue()
    {
        var args = GetNamedArgs(nameof(AttributedMethods.WithSchemeOnly));

        args.IsAuthenticationSchemeVerificationOnly().Should().BeTrue();
    }

    [Test]
    public void IsAuthenticationSchemeVerificationOnly_WhenSchemeAndOtherArgs_ReturnsFalse()
    {
        var args = GetNamedArgs(nameof(AttributedMethods.WithSchemeAndRoles));

        args.IsAuthenticationSchemeVerificationOnly().Should().BeFalse();
    }

    [Test]
    public void IsAuthenticationSchemeVerificationOnly_WhenOnlyRoleArgs_ReturnsFalse()
    {
        var args = GetNamedArgs(nameof(AttributedMethods.WithRoles));

        args.IsAuthenticationSchemeVerificationOnly().Should().BeFalse();
    }

    [Test]
    public void ToMemberValueDictionary_WhenArgsExist_ReturnsDictionaryWithMemberNameKeys()
    {
        var args = GetNamedArgs(nameof(AttributedMethods.WithRoles));

        var dict = args.ToMemberValueDictionary();

        dict.Should().ContainKey("Roles").WhoseValue.Should().Be("Admin");
    }
}
