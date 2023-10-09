using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace CardboardBox.Json;

/// <summary>
/// Extensions for registering the JsonService
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Registers the json service using the default System.Text.Json implementation
    /// </summary>
    /// <param name="services">The service collection to register with</param>
    /// <param name="options">The serialization options</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddJson(this IServiceCollection services, JsonSerializerOptions? options = null)
    {
        return services.AddJson(new SystemTextJsonService(options ?? new JsonSerializerOptions()));
    }

    /// <summary>
    /// Registers the json service using the given <see cref="IJsonService"/> implementation
    /// </summary>
    /// <typeparam name="T">The type of <see cref="IJsonService"/></typeparam>
    /// <param name="services">The service collection to register with</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddJson<T>(this IServiceCollection services) where T : class, IJsonService
    {
        return services.AddTransient<IJsonService, T>();
    }

    /// <summary>
    /// Registers the given json service 
    /// </summary>
    /// <param name="services">The service collection to register with</param>
    /// <param name="json">The implementation of the <see cref="IJsonService"/></param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddJson(this IServiceCollection services, IJsonService json)
    {
        return services.AddSingleton(json);
    }
}
