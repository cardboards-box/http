namespace CardboardBox.Http;

/// <summary>
/// Exposes some common HTTP methods using <see cref="IHttpClientFactory"/> with built in features like caching, authorization, JSON deserializing, etc.
/// </summary>
public interface IApiService
{
    /// <summary>
    /// Creates an <see cref="IHttpBuilder"/> for the given URL and method
    /// </summary>
    /// <param name="url">The url to request data from</param>
    /// <param name="json">The JSON provider to use for this request</param>
    /// <param name="method">The method used for this HTTP request</param>
    /// <returns>An instance of the <see cref="IHttpBuilder"/></returns>
    IHttpBuilder Create(string url, IJsonService json, string method);

    /// <summary>
    /// Creates an <see cref="IHttpBuilder"/> for the given URL and method
    /// </summary>
    /// <param name="url">The url to request data from</param>
    /// <param name="method">The method used for this HTTP request</param>
    /// <param name="config">Any optional configuration necessary</param>
    /// <param name="token">The cancellation token for the request</param>
    /// <returns>An instance of the <see cref="IHttpBuilder"/></returns>
    IHttpBuilder Create(string url, string method, Action<IHttpBuilderConfig>? config, CancellationToken? token);
}

/// <summary>
/// The concrete implementation of <see cref="IApiService"/>
/// </summary>
/// <param name="_factory">The factory for creating the HTTP client</param>
/// <param name="_json">The service for parsing JSON</param>
public class ApiService(
    IHttpClientFactory _factory,
    IJsonService _json) : IApiService
{
    /// <summary>
    /// Creates an <see cref="IHttpBuilder"/> for the given URL and method
    /// </summary>
    /// <param name="url">The url to request data from</param>
    /// <param name="json">The JSON provider to use for this request</param>
    /// <param name="method">The method used for this HTTP request</param>
    /// <returns>An instance of the <see cref="IHttpBuilder"/></returns>
    public virtual IHttpBuilder Create(string url, IJsonService json, string method) 
        => (IHttpBuilder)new HttpBuilder(_factory, json).Method(method).Uri(url);

    /// <summary>
    /// Creates an <see cref="IHttpBuilder"/> for the given URL and method
    /// </summary>
    /// <param name="url">The url to request data from</param>
    /// <param name="method">The method used for this HTTP request</param>
    /// <param name="config">Any optional configuration necessary</param>
    /// <param name="token">The cancellation token for the request</param>
    /// <returns>An instance of the <see cref="IHttpBuilder"/></returns>
    public virtual IHttpBuilder Create(string url, string method, Action<IHttpBuilderConfig>? config, CancellationToken? token) 
        => (IHttpBuilder)Create(url, _json, method).CancelWith(token).With(config);
}
