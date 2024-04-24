using Microsoft.AspNetCore.Builder;
using Swashbuckle.AspNetCore.SwaggerUI;
using SwaggerUIAuthorization.Components;

namespace SwaggerUIAuthorization.Extensions;

public static class IApplicationBuilderExtensions
{
    private static SwaggerUIOptions GetSwaggerUIOptions(
        Action<SwaggerUIOptions>? configureOptions = null
    )
    {
        var options = new SwaggerUIOptions();
        configureOptions?.Invoke(options);
        
        // Authorized schema is not yet supported
        options.DefaultModelsExpandDepth(-1);

        return options;
    }

    private static IApplicationBuilder UseSwaggerAuthentication(
        this IApplicationBuilder app,
        Action<SwaggerAuthenticationOptions>? configureOptions = null
    ) 
    {
        var options = new SwaggerAuthenticationOptions();
        configureOptions?.Invoke(options);

        return app.UseMiddleware<SwaggerAuthenticationMiddleware>(options);
    }

    /// <summary>
    /// Registers an instance of <c>SwaggerAuthenticationMiddleware</c> with the default authentication scheme. 
    /// <c>SwaggerAuthenticationMiddleware</c> handles challenging unauthenticated swagger routes.
    /// <c>UseSwaggerUI</c> is also called with the default options.
    /// </summary>
    /// <param name="app">An <c>IApplicationBuilder</c>.</param>
    /// <returns>An <c>IApplicationBuilder</c>.</returns>
    public static IApplicationBuilder UseSwaggerUIAuthorization(
        this IApplicationBuilder app
    ) => app.UseSwaggerAuthentication()
            .UseSwaggerUI(GetSwaggerUIOptions());

    /// <summary>
    /// Registers an instance of <c>SwaggerAuthenticationMiddleware</c> with the specified authentication scheme. 
    /// <c>SwaggerAuthenticationMiddleware</c> handles challenging unauthenticated swagger routes.
    /// <c>UseSwaggerUI</c> is also called with the default options.
    /// </summary>
    /// <param name="app">An <c>IApplicationBuilder</c>.</param>
    /// <param name="authenticationScheme">A <c>string</c> representing the name of the authentication scheme to use.</param>
    /// <returns>An <c>IApplicationBuilder</c>.</returns>
    public static IApplicationBuilder UseSwaggerUIAuthorization(
        this IApplicationBuilder app,
        string authenticationScheme
    ) => app.UseSwaggerAuthentication(options => options.AuthenticationScheme = authenticationScheme)
            .UseSwaggerUI(GetSwaggerUIOptions());

    /// <summary>
    /// Registers an instance of <c>SwaggerAuthenticationMiddleware</c> with the specified authentication scheme. 
    /// <c>SwaggerAuthenticationMiddleware</c> handles challenging unauthenticated swagger routes.
    /// <c>UseSwaggerUI</c> is also called with the specified options.
    /// </summary>
    /// <param name="app">An <c>IApplicationBuilder</c>.</param>
    /// <param name="authenticationScheme">A <c>string</c> representing the name of the authentication scheme to use.</param>
    /// <param name="configureOptions">An <c>Action</c> of <c>SwaggerUIOptions</c> used to configure SwaggerUI.</param>
    /// <returns>An <c>IApplicationBuilder</c>.</returns>
    public static IApplicationBuilder UseSwaggerUIAuthorization(
        this IApplicationBuilder app, 
        string authenticationScheme,
        Action<SwaggerUIOptions> configureOptions
    )
    {
        var options = GetSwaggerUIOptions(configureOptions);
        return app
            .UseSwaggerAuthentication(authOptions =>
            {
                authOptions.RoutePrefix = options.RoutePrefix;
                authOptions.AuthenticationScheme = authenticationScheme;
            })
            .UseSwaggerUI(options);
    }

    /// <summary>
    /// Registers an instance of <c>SwaggerAuthenticationMiddleware</c> with the default authentication scheme. 
    /// <c>SwaggerAuthenticationMiddleware</c> handles challenging unauthenticated swagger routes.
    /// <c>UseSwaggerUI</c> is also called with the specified options.
    /// </summary>
    /// <param name="app">An <c>IApplicationBuilder</c>.</param>
    /// <param name="configureOptions">An <c>Action</c> of <c>SwaggerUIOptions</c> used to configure SwaggerUI.</param>
    /// <returns>An <c>IApplicationBuilder</c>.</returns>
    public static IApplicationBuilder UseSwaggerUIAuthorization(
        this IApplicationBuilder app,
        Action<SwaggerUIOptions> configureOptions
    )
    {
        var options = GetSwaggerUIOptions(configureOptions);
        return app
            .UseSwaggerAuthentication(authOptions => authOptions.RoutePrefix = options.RoutePrefix)
            .UseSwaggerUI(options);
    }
}