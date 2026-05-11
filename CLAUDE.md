# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

`SwaggerUIAuthorization` is a .NET 8 NuGet library (currently v1.0.4) that restricts Swagger/SwaggerUI access based on the `AuthorizeAttribute` rules already defined in an ASP.NET Core application. It integrates with Swashbuckle's filter pipeline to conditionally show or hide API endpoints in the generated OpenAPI document.

## Build and Development Commands

```bash
# Build the library
dotnet build src/SwaggerUIAuthorization.csproj

# Pack the NuGet package
dotnet pack src/SwaggerUIAuthorization.csproj

# Run a sample project
dotnet run --project samples/WebApiWithCustomLogin/src/WebApiWithCustomLogin.csproj
dotnet run --project samples/WebApiWithMicrosoftIdentity/src/WebApiWithMicrosoftIdentity.csproj
```

There are no automated tests in this repository.

## Architecture

The library works by inserting two Swashbuckle filters and a middleware into the ASP.NET Core pipeline:

### Request-time authentication (middleware)
`SwaggerAuthenticationMiddleware` intercepts requests to the Swagger route prefix and challenges unauthenticated users via `ChallengeAsync`. It runs before `UseSwaggerUI`.

### Document generation (Swashbuckle filters)
Two filters run during document generation (`/swagger/{doc}/swagger.json` requests):

1. **`SwaggerAccessOperationFilter`** (`IOperationFilter`) — runs per-operation. It inspects the action's `AuthorizeAttribute` and `AllowAnonymousAttribute` via reflection, evaluates them against the current user, and adds passing operations to `SwaggerOperationCollection`.

2. **`SwaggerAccessDocumentFilter`** (`IDocumentFilter`) — runs after all operation filters. It removes any path operations that were not added to `SwaggerOperationCollection` by the operation filter.

### Authorization evaluation
- **`SwaggerAuthorizationHandler`** — decides `ShouldRender` for a single `CustomAttributeData`. Handles both `ConstructorArguments` (policy string) and `NamedArguments` (Roles, Policy, AuthenticationSchemes). Multiple `AuthorizeAttribute`s on a method are AND-ed together.
- **`SwaggerAuthorizationProvider`** — checks roles via `ClaimsPrincipal.IsInRole` and policies via `IAuthorizationService.AuthorizeAsync` (called synchronously via `.Result`).
- **`SwaggerOperationCollection`** — a singleton `HashSet<string>` keyed by Swashbuckle action ID (injected into the operation tag and used as the correlation key between the two filters).

### Public API surface
Two extension methods are the only public entry points:
- `IServiceCollection.AddSwaggerUIAuthorization()` — registers DI services and injects the two filters into `AddSwaggerGen`.
- `IApplicationBuilder.UseSwaggerUIAuthorization(scheme?, configureOptions?)` — registers `SwaggerAuthenticationMiddleware` then calls `UseSwaggerUI`. Must be placed after `UseAuthentication` and before `UseAuthorization`.

### Key constraints
- `DefaultModelsExpandDepth(-1)` is always forced — schema models at the bottom of SwaggerUI are always hidden (authorized schema display is not yet supported).
- If an endpoint's `AuthorizeAttribute` specifies an authentication scheme different from the one passed to `UseSwaggerUIAuthorization`, the endpoint is not challenged and will not render.
- Comma-separated roles in a single `AuthorizeAttribute` are evaluated OR; multiple `AuthorizeAttribute`s are AND-ed.

### Internal extensions (`src/Extensions/Internal/`)
These are not part of the public API. They contain reflection helpers for reading `CustomAttributeNamedArgument` / `CustomAttributeTypedArgument` values (roles, policies, schemes), `HttpContext` helpers for route-prefix matching and user extraction, and `OpenApiTag` helpers for embedding/retrieving the action ID correlation key.
