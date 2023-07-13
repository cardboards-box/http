using Microsoft.Extensions.DependencyInjection;

namespace CardboardBox.Http;

/// <summary>
/// Dependency Injection extensions for the <see cref="IFileCacheService"/>
/// </summary>
public static class DiExtensions
{
    /// <summary>
    /// Registers the file cache service with the dependency injection system
    /// </summary>
    /// <param name="services">The services</param>
    /// <returns>The services (for fluent chaining)</returns>
    public static IServiceCollection AddFileCache(this IServiceCollection services)
    {
        return services.AddTransient<IFileCacheService, FileCacheService>();
    }
}
