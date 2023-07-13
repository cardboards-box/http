namespace CardboardBox.Http;

/// <summary>
/// Extensions that include Cardboard HTTP in the given <see cref="IServiceCollection"/>
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Adds Cardboard HTTP with the given JSON provider
    /// </summary>
    /// <param name="services">The service collection to add Cardboard HTTP to</param>
    /// <returns>The referenced service provider for chaining</returns>
    public static IServiceCollection AddCardboardHttp(this IServiceCollection services)
    {
        return services
            .AddCardboardHttpBase();
    }


    /// <summary>
    /// Registers all of the base services necessary for Cardboard HTTP
    /// </summary>
    /// <param name="services">The service collection to add Cardboard HTTP to</param>
    /// <returns>The referenced service provider for chaining</returns>
    private static IServiceCollection AddCardboardHttpBase(this IServiceCollection services)
    {
        return services
            .AddHttpClient()
            .AddTransient<IApiService, ApiService>()
            .AddTransient<ICacheService, DiskCacheService>();
    }
}