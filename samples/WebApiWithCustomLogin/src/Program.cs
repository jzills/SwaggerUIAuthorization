using System.Security.Claims;
using SwaggerUIAuthorization.Extensions;
using Sample.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAuthentication(AuthenticationDefaults.AuthenticationScheme)
    .AddBearerToken("MyBearerToken", options =>
    {
        options.ForwardChallenge = AuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(AuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/authentication/login";
    });

builder.Services
    .AddAuthorization(options => options
        .AddPolicy("CanDeletePolicy", policy => 
        {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim(ClaimTypes.NameIdentifier);
            policy.AddAuthenticationSchemes(AuthenticationDefaults.AuthenticationScheme);
        }));

builder.Services.AddHttpContextAccessor();
builder.Services.AddMvc();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerUIAuthorization();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();

// Custom route
app.UseSwagger(options =>
{
    options.RouteTemplate = "mycoolapi/{documentname}/swagger.json";
});

app.UseSwaggerUIAuthorization(AuthenticationDefaults.AuthenticationScheme, options => 
{
    options.SwaggerEndpoint("/mycoolapi/v1/swagger.json", "My Cool Api V1");
    options.RoutePrefix = "mycoolapi";
});

app.UseReDoc(options => 
{
    options.SpecUrl = "/mycoolapi/v1/swagger.json";
    options.RoutePrefix = "mycoolapi/docs";
});

app.UseAuthorization();

app.MapControllers();

app.Run();
