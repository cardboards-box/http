using Microsoft.Extensions.DependencyInjection;

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
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddNewtonsoftJson(this IServiceCollection services)
    {
        return services.AddJson<NewtonsoftJsonService>();
    }
}
