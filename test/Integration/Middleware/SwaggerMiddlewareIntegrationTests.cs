using System.Net;
using FluentAssertions;
using NUnit.Framework;
using SwaggerUIAuthorization.Integration.Tests.Infrastructure;

namespace SwaggerUIAuthorization.Integration.Tests.Middleware;

[TestFixture]
public class SwaggerMiddlewareIntegrationTests
{
    private TestWebApplicationFactory _factory = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp() => _factory = new TestWebApplicationFactory();

    [OneTimeTearDown]
    public void OneTimeTearDown() => _factory.Dispose();

    private HttpClient CreateUnauthenticatedClient() => _factory.CreateClient();

    private HttpClient CreateAuthenticatedClient(string role = "Admin")
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(TestAuthHandler.RoleHeader, role);
        return client;
    }

    [Test]
    public async Task SwaggerUi_WhenUnauthenticated_Returns401()
    {
        var client = CreateUnauthenticatedClient();

        var response = await client.GetAsync("/swagger/index.html");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task SwaggerUi_WhenAuthenticated_Returns200()
    {
        var client = CreateAuthenticatedClient();

        var response = await client.GetAsync("/swagger/index.html");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task SwaggerUi_WhenAuthenticatedWithAnyRole_Returns200()
    {
        // The challenge middleware only checks authentication, not authorization.
        // Role-based visibility is enforced later by the document filter.
        var client = CreateAuthenticatedClient(role: "NonExistentRole");

        var response = await client.GetAsync("/swagger/index.html");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task SwaggerJson_WhenAuthenticated_Returns200()
    {
        var client = CreateAuthenticatedClient();

        var response = await client.GetAsync("/swagger/v1/swagger.json");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task SwaggerJson_WhenUnauthenticated_IsAccessibleBecauseItPrecedesMiddleware()
    {
        // UseSwagger() is registered before UseSwaggerUIAuthorization(), so the swagger.json
        // endpoint is served before the challenge middleware runs. Document-level filtering
        // still applies — only authorized endpoints appear in the response.
        var client = CreateUnauthenticatedClient();

        var response = await client.GetAsync("/swagger/v1/swagger.json");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task NonSwaggerRoute_WhenUnauthenticated_IsNotChallenged()
    {
        var client = CreateUnauthenticatedClient();

        var response = await client.GetAsync("/api/open");

        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
    }
}
