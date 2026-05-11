using System.Text.Json;
using FluentAssertions;
using NUnit.Framework;
using SwaggerUIAuthorization.Integration.Tests.Infrastructure;

namespace SwaggerUIAuthorization.Integration.Tests.Filters;

[TestFixture]
public class SwaggerDocumentFilterIntegrationTests
{
    // HTTP method keys as defined by the OpenAPI specification.
    private static readonly HashSet<string> HttpMethodKeys =
        new(StringComparer.OrdinalIgnoreCase) { "get", "post", "put", "delete", "patch", "options", "head", "trace" };

    private TestWebApplicationFactory _factory = null!;

    // A fresh factory per test is required because SwaggerOperationCollection is a singleton
    // that accumulates state across swagger.json requests without being cleared. Each test
    // needs a clean DI container so that document filter results reflect only the current user.
    [SetUp]
    public void SetUp() => _factory = new TestWebApplicationFactory();

    [TearDown]
    public void TearDown() => _factory.Dispose();

    private async Task<IReadOnlyList<string>> GetSwaggerPathsAsync(params string[] roles)
    {
        var client = _factory.CreateClient();

        if (roles.Length > 0)
            client.DefaultRequestHeaders.Add(TestAuthHandler.RoleHeader, string.Join(",", roles));

        var response = await client.GetAsync("/swagger/v1/swagger.json");
        response.EnsureSuccessStatusCode();

        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        // Filter to paths that have at least one HTTP method operation. The document filter
        // removes operations from path items but does not remove the path keys themselves,
        // so paths with all operations removed will appear as empty objects in the JSON.
        return doc.RootElement
            .GetProperty("paths")
            .EnumerateObject()
            .Where(path => path.Value.EnumerateObject().Any(op => HttpMethodKeys.Contains(op.Name)))
            .Select(path => path.Name)
            .ToArray();
    }

    [Test]
    public async Task SwaggerJson_WhenAdminRole_ShowsAdminEndpoint()
    {
        var paths = await GetSwaggerPathsAsync("Admin");

        paths.Should().Contain("/api/admin");
    }

    [Test]
    public async Task SwaggerJson_WhenUserRole_HidesAdminEndpoint()
    {
        var paths = await GetSwaggerPathsAsync("User");

        paths.Should().NotContain("/api/admin");
    }

    [Test]
    public async Task SwaggerJson_WhenUserRole_ShowsUserEndpoint()
    {
        var paths = await GetSwaggerPathsAsync("User");

        paths.Should().Contain("/api/user");
    }

    [Test]
    public async Task SwaggerJson_WhenAdminRole_HidesUserEndpoint()
    {
        var paths = await GetSwaggerPathsAsync("Admin");

        paths.Should().NotContain("/api/user");
    }

    [Test]
    public async Task SwaggerJson_ForAnyAuthenticatedUser_ShowsAnonymousEndpoint()
    {
        var paths = await GetSwaggerPathsAsync("AnyRole");

        paths.Should().Contain("/api/anonymous");
    }

    [Test]
    public async Task SwaggerJson_ForAnyAuthenticatedUser_ShowsOpenEndpoint()
    {
        var paths = await GetSwaggerPathsAsync("AnyRole");

        paths.Should().Contain("/api/open");
    }

    [Test]
    public async Task SwaggerJson_WhenAuthenticated_ShowsPolicyEndpoint()
    {
        // TestPolicy requires RequireAuthenticatedUser + RequireClaim(NameIdentifier).
        // TestAuthHandler injects NameIdentifier for any authenticated request.
        var paths = await GetSwaggerPathsAsync("AnyRole");

        paths.Should().Contain("/api/policy");
    }

    [Test]
    public async Task SwaggerJson_WhenUnauthenticated_HidesPolicyEndpoint()
    {
        // swagger.json is accessible without auth (UseSwagger precedes the challenge middleware),
        // but the document filter evaluates policy against the anonymous user, which fails.
        var paths = await GetSwaggerPathsAsync();

        paths.Should().NotContain("/api/policy");
    }

    [Test]
    public async Task SwaggerJson_WhenUnauthenticated_HidesAdminEndpoint()
    {
        var paths = await GetSwaggerPathsAsync();

        paths.Should().NotContain("/api/admin");
    }

    [Test]
    public async Task SwaggerJson_WhenUnauthenticated_StillShowsOpenEndpoint()
    {
        var paths = await GetSwaggerPathsAsync();

        paths.Should().Contain("/api/open");
    }

    [Test]
    public async Task SwaggerJson_WhenUnauthenticated_StillShowsAnonymousEndpoint()
    {
        var paths = await GetSwaggerPathsAsync();

        paths.Should().Contain("/api/anonymous");
    }

    [Test]
    public async Task SwaggerJson_WhenAdminRole_ShowsMultiRoleEndpoint()
    {
        // [Authorize(Roles = "Admin,User")] uses OR semantics: Admin satisfies it.
        var paths = await GetSwaggerPathsAsync("Admin");

        paths.Should().Contain("/api/multirole");
    }

    [Test]
    public async Task SwaggerJson_WhenUserRole_ShowsMultiRoleEndpoint()
    {
        // [Authorize(Roles = "Admin,User")] uses OR semantics: User satisfies it.
        var paths = await GetSwaggerPathsAsync("User");

        paths.Should().Contain("/api/multirole");
    }

    [Test]
    public async Task SwaggerJson_WhenUnrelatedRole_HidesMultiRoleEndpoint()
    {
        var paths = await GetSwaggerPathsAsync("Guest");

        paths.Should().NotContain("/api/multirole");
    }
}
