using Microsoft.Identity.Web;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using SwaggerUIAuthorization.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerUIAuthorization();

var app = builder.Build();

app.UseSwagger(options =>
{
    options.RouteTemplate = "mycoolapi/{documentname}/swagger.json";
});

app.UseHttpsRedirection();

app.UseAuthentication();

// The call to UseSwaggerUIAuthorization must come after
// the call to UseAuthentication. The reason for this is because
// UseSwaggerUIAuthorization relies on having access to an authenticated
// user to determine authorization rules. If you place this before UseAuthentication,
// the authentication scheme will be infinitely challenged. 
app.UseSwaggerUIAuthorization(OpenIdConnectDefaults.AuthenticationScheme, options => 
{
    options.SwaggerEndpoint("/mycoolapi/v1/swagger.json", "My Cool Api V1");
    options.RoutePrefix = "mycoolapi";
});

app.UseAuthorization();

app.MapControllers();

app.Run();
