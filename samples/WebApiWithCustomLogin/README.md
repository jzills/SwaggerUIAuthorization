
# WebApiWithCustomLogin

[![NuGet Version](https://img.shields.io/nuget/v/SwaggerUIAuthorization.svg)](https://www.nuget.org/packages/SwaggerUIAuthorization/) [![NuGet Downloads](https://img.shields.io/nuget/dt/SwaggerUIAuthorization.svg)](https://www.nuget.org/packages/SwaggerUIAuthorization/)

## Usage

- Register an authentication scheme, in this case, we use a custom scheme named `SomeAuthenticationScheme` or `AuthenticationDefaults.AuthenticationScheme`
- Additionally, we've registered a policy named `CanDeletePolicy` with `AddAuthorization` to ensure only users with this policy can view the documentation for the `DELETE` action in the `UserController`.

## Example

    builder.Services.AddAuthentication(AuthenticationDefaults.AuthenticationScheme);

    builder.Services.AddAuthorization(options => options.AddPolicy("CanDeletePolicy", ...));

    builder.Services.AddSwaggerUIAuthorization();

    var app = builder.Build();

    app.UseAuthentication();

    app.UseSwagger(...);

    app.UseSwaggerUIAuthorization(AuthenticationDefaults.AuthenticationScheme, ...);

    app.UseReDoc(...);

    app.UseAuthorization();

## Docs

[SwaggerUIAuthorization](../../src/README.md)