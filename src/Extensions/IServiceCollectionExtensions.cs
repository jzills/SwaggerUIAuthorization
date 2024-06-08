using Microsoft.Extensions.DependencyInjection;
using SwaggerUIAuthorization.Components;
using SwaggerUIAuthorization.Extensions.Internal;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SwaggerUIAuthorization.Extensions;

/// <summary>
/// An <c>class</c> representing <c>IServiceCollection</c> extension methods.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Registers required dependencies into the DI container.
    /// </summary>
    /// <param name="services">An <c>IServiceCollection</c>.</param>
    /// <returns>An <c>IServiceCollection</c>.</returns>
    private static IServiceCollection AddSwaggerUIAuthorizationDefaults(
        this IServiceCollection services
    ) => services
            .AddHttpContextAccessor()
            .AddSingleton<ISwaggerOperationCollection, SwaggerOperationCollection>()
            .AddTransient<ISwaggerAuthorizationHandler, SwaggerAuthorizationHandler>()
            .AddTransient<ISwaggerAuthorizationProvider, SwaggerAuthorizationProvider>();
        
    /// <summary>
    /// Registers the required components for SwaggerUIAuthorization. 
    /// <c>AddSwaggerGen</c> is also called with the default options.
    /// </summary>
    /// <param name="services">An <c>IServiceCollection</c>.</param>
    /// <returns>An <c>IServiceCollection</c>.</returns>
    public static IServiceCollection AddSwaggerUIAuthorization(
        this IServiceCollection services
    ) => services
            .AddSwaggerUIAuthorizationDefaults()
            .AddSwaggerGen(options => options.AddSwaggerAuthorizationFilters());

    /// <summary>
    /// Registers the required components for SwaggerUIAuthorization. 
    /// <c>AddSwaggerGen</c> is also called with the specified options.
    /// </summary>
    /// <param name="services">An <c>IServiceCollection</c>.</param>
    /// /// <param name="services">An <c>Action</c> of <c>SwaggerGenOptions</c>.</param>
    /// <returns>An <c>IServiceCollection</c>.</returns>
    public static IServiceCollection AddSwaggerUIAuthorization(
        this IServiceCollection services,
        Action<SwaggerGenOptions> configureOptions
    )
    {
        services.AddSwaggerUIAuthorizationDefaults();

        var options = new SwaggerGenOptions();
        configureOptions.Invoke(options);

        options.AddSwaggerAuthorizationFilters();

        return services.AddSwaggerGen(_ => configureOptions(options));
    }
}