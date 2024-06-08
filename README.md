
# SwaggerUIAuthorization

[![NuGet Version](https://img.shields.io/nuget/v/SwaggerUIAuthorization.svg)](https://www.nuget.org/packages/SwaggerUIAuthorization/) [![NuGet Downloads](https://img.shields.io/nuget/dt/SwaggerUIAuthorization.svg)](https://www.nuget.org/packages/SwaggerUIAuthorization/)

## Summary

If your API endpoints require authentication and authorization, it makes sense to integrate those rules into your Swagger documentation. With the `SwaggerUIAuthorization` package, you can seamlessly control access to SwaggerUI, ensuring that only authenticated users with appropriate permissions can view documentation their roles have access to. This package leverages .NET's authentication and authorization mechanisms, aligning with the rules already established within your application.

## Features

- Authentication and authorization dependent rendering of swagger documentation
- The same rules that .NET has defined for an `AuthorizeAttribute` applies
    - Comma separated roles are evaluated on an OR basis
    - Multiple `AuthorizeAttribute`'s are evaluated on an AND basis
    - An `AllowAnonymousAttribute` bypasses all authorization

## Limitations

The schema displayed at the bottom of an api using SwaggerUI is force hidden internally using the following code:
    
    options.DefaultModelsExpandDepth(-1);

## Installation

`SwaggerUIAuthorization` is available on [NuGet](https://www.nuget.org/packages/SwaggerUIAuthorization/). 

    dotnet add package SwaggerUIAuthorization

## Docs

[SwaggerUIAuthorization](src/README.md)

## Examples

[WebApiWithCustomLogin](samples/WebApiWithCustomLogin/README.md) \
[WebApiWithMicrosoftIdentity](samples/WebApiWithMicrosoftIdentity/README.md)