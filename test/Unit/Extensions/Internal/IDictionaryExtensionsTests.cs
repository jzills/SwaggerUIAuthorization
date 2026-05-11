using FluentAssertions;
using NUnit.Framework;
using SwaggerUIAuthorization.Extensions.Internal;

namespace SwaggerUIAuthorization.Extensions.Internal.Tests;

[TestFixture]
public class IDictionaryExtensionsTests
{
    [Test]
    public void TryGetRoleValue_WhenRolesKeyExists_ReturnsTrueAndValue()
    {
        var source = new Dictionary<string, string> { ["Roles"] = "Admin" };

        var result = source.TryGetRoleValue(out var roleValue);

        result.Should().BeTrue();
        roleValue.Should().Be("Admin");
    }

    [Test]
    public void TryGetRoleValue_WhenRolesKeyDoesNotExist_ReturnsFalse()
    {
        var source = new Dictionary<string, string> { ["Policy"] = "AdminPolicy" };

        var result = source.TryGetRoleValue(out var roleValue);

        result.Should().BeFalse();
        roleValue.Should().BeNull();
    }

    [TestCase("")]
    [TestCase("   ")]
    public void TryGetRoleValue_WhenRolesValueIsNullOrWhitespace_ReturnsFalse(string value)
    {
        var source = new Dictionary<string, string> { ["Roles"] = value };

        var result = source.TryGetRoleValue(out var roleValue);

        result.Should().BeFalse();
    }

    [Test]
    public void TryGetPolicyValue_WhenPolicyKeyExists_ReturnsTrueAndValue()
    {
        var source = new Dictionary<string, string> { ["Policy"] = "AdminPolicy" };

        var result = source.TryGetPolicyValue(out var policyValue);

        result.Should().BeTrue();
        policyValue.Should().Be("AdminPolicy");
    }

    [Test]
    public void TryGetPolicyValue_WhenPolicyKeyDoesNotExist_ReturnsFalse()
    {
        var source = new Dictionary<string, string> { ["Roles"] = "Admin" };

        var result = source.TryGetPolicyValue(out var policyValue);

        result.Should().BeFalse();
        policyValue.Should().BeNull();
    }

    [TestCase("")]
    [TestCase("   ")]
    public void TryGetPolicyValue_WhenPolicyValueIsNullOrWhitespace_ReturnsFalse(string value)
    {
        var source = new Dictionary<string, string> { ["Policy"] = value };

        var result = source.TryGetPolicyValue(out var policyValue);

        result.Should().BeFalse();
    }

    [Test]
    public void TryGetRoleValue_WhenDictionaryIsEmpty_ReturnsFalse()
    {
        var source = new Dictionary<string, string>();

        var result = source.TryGetRoleValue(out _);

        result.Should().BeFalse();
    }

    [Test]
    public void TryGetPolicyValue_WhenDictionaryIsEmpty_ReturnsFalse()
    {
        var source = new Dictionary<string, string>();

        var result = source.TryGetPolicyValue(out _);

        result.Should().BeFalse();
    }
}
