using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using SwaggerUIAuthorization.Extensions;

namespace SwaggerUIAuthorization.Integration.Tests.Infrastructure;

public class TestWebApplicationFactory : IDisposable
{
    private readonly TestServer _server;

    public TestWebApplicationFactory()
    {
        _server = new TestServer(
            new WebHostBuilder()
                .ConfigureServices(ConfigureServices)
                .Configure(ConfigureApp)
        );
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services
            .AddAuthentication(TestAuthHandler.SchemeName)
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                TestAuthHandler.SchemeName, _ => { });

        services.AddAuthorization(options =>
            options.AddPolicy("TestPolicy", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(ClaimTypes.NameIdentifier);
            }));

        services.AddControllers()
            .AddApplicationPart(Assembly.GetExecutingAssembly());

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Test API", Version = "v1" }));

        services.AddSwaggerUIAuthorization();
    }

    private static void ConfigureApp(IApplicationBuilder app)
    {
        app.UseRouting();
        app.UseAuthentication();

        // UseSwagger must come before UseSwaggerUIAuthorization so that the challenge
        // middleware (registered by UseSwaggerUIAuthorization) intercepts swagger UI
        // paths but swagger.json remains accessible for document filter testing.
        app.UseSwagger();

        // Use the scheme + options overload so that SwaggerAuthenticationOptions.RoutePrefix
        // correctly syncs from SwaggerUIOptions.RoutePrefix ("swagger"), ensuring the challenge
        // middleware path check produces "/swagger" rather than "//swagger".
        app.UseSwaggerUIAuthorization(TestAuthHandler.SchemeName, options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Test API v1");
        });

        app.UseAuthorization();
        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }

    public HttpClient CreateClient() => _server.CreateClient();

    public void Dispose() => _server.Dispose();
}
