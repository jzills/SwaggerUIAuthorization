
# WebApiWithMicrosoftIdentity

[![NuGet Version](https://img.shields.io/nuget/v/SwaggerUIAuthorization.svg)](https://www.nuget.org/packages/SwaggerUIAuthorization/) [![NuGet Downloads](https://img.shields.io/nuget/dt/SwaggerUIAuthorization.svg)](https://www.nuget.org/packages/SwaggerUIAuthorization/)

## Usage

- Register `AddMicrosoftIdentityWebApp` with your credentials
- Register `SwaggerUI` through the `AddSwaggerUIAuthorization()` extension method
- Call `UseSwaggerUIAuthorization` using an authentication scheme of `OpenIdConnect` or `OpenIdConnectDefaults.AuthenticationScheme`
    - The above call must be placed after `UseAuthentication()` and before `UseAuthorization()` otherwise an infinite loop will occur attempting to authenticate a user before the claims have been created

## Example

    builder.Services
        .AddAuthentication(OpenIdConnectDefaultsAuthenticationScheme)
        .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

    builder.Services.AddSwaggerUIAuthorization();

    var app = builder.Build();

    app.UseSwagger(...);

    app.UseHttpsRedirection();

    app.UseAuthentication();

    app.UseSwaggerUIAuthorization(OpenIdConnectDefaults.AuthenticationScheme, options => ...);

    app.UseAuthorization();

## Docs

[SwaggerUIAuthorization](../../src/README.md)