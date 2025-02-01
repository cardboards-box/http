using Flurl;

namespace CardboardBox.Http;

/// <summary>
/// A set of extensions for configuring <see cref="IHttpBuilder"/>s
/// </summary>
public static class HttpBuilderExtensions
{
    /// <summary>
    /// Sets the Accept header to the given value
    /// </summary>
    /// <param name="builder">The builder to configure</param>
    /// <param name="accept">The accept header's value</param>
    /// <returns>The instance of <see cref="IHttpBuilderConfig"/> for chaining</returns>
    public static IHttpBuilderConfig Accept(this IHttpBuilderConfig builder, string accept)
    {
        return builder.Message(c => c.Headers.Add("Accept", accept));
    }

    /// <summary>
    /// Sets the authorization header for the request to the given token and scheme
    /// </summary>
    /// <param name="builder">The builder to configure</param>
    /// <param name="token">The token to use</param>
    /// <param name="scheme">The scheme to use (defaults to "Bearer")</param>
    /// <returns>The instance of <see cref="IHttpBuilder"/> for chaining</returns>
    public static IHttpBuilderConfig Authorization(this IHttpBuilderConfig builder, string token, string scheme = "Bearer")
    {
        return builder.Message(c => c.Headers.Add("Authorization", $"{scheme} {token}"));
    }

    /// <summary>
    /// Sets the body content of the request
    /// </summary>
    /// <param name="builder">The builder to configure</param>
    /// <param name="content">The body content</param>
    /// <returns>The instance of <see cref="IHttpBuilder"/> for chaining</returns>
    public static IHttpBuilderConfig BodyContent(this IHttpBuilderConfig builder, HttpContent content)
    {
        return builder.Message(c => c.Content = content);
    }

    /// <summary>
    /// Sets the body content to a <see cref="FormUrlEncodedContent"/> built from the given parameters
    /// </summary>
    /// <param name="builder">The builder to configure</param>
    /// <param name="data">The parameters to be encoded</param>
    /// <returns>The instance of <see cref="IHttpBuilder"/> for chaining</returns>
    public static IHttpBuilderConfig Body(this IHttpBuilderConfig builder, params (string, string)[] data)
    {
        var kvp = data.Select(t => new KeyValuePair<string, string>(t.Item1, t.Item2));
        var content = new FormUrlEncodedContent(kvp);
        return builder.BodyContent(content);
    }

    /// <summary>
    /// Sets the body content to the given JSON serialized object
    /// </summary>
    /// <typeparam name="T">The type of JSON object to send</typeparam>
    /// <param name="builder">The builder to configure</param>
    /// <param name="data">The data to serialize</param>
    /// <returns>The instance of <see cref="IHttpBuilder"/> for chaining</returns>
    public static IHttpBuilderConfig Body<T>(this IHttpBuilderConfig builder, T data)
    {
        var str = builder.JsonService.Serialize(data);
        var json = new StringContent(str, Encoding.UTF8, "application/json");
        return builder.BodyContent(json);
    }

    /// <summary>
    /// Instead of throwing an exception, it will log the error and return a default value
    /// </summary>
    /// <param name="builder">The builder to configure</param>
    /// <returns>The instance of <see cref="IHttpBuilderConfig"/> for chaining</returns>
    public static IHttpBuilderConfig FailGracefully(this IHttpBuilderConfig builder)
    {
        return builder.ThrowOnNull(false);
    }

    /// <summary>
    /// Throw an exception if the returned status code isn't in the 200 range.
    /// </summary>
    /// <param name="builder">The builder to configure</param>
    /// <returns>The instance of <see cref="IHttpBuilderConfig"/> for chaining</returns>
    public static IHttpBuilderConfig FailWithThrow(this IHttpBuilderConfig builder)
    {
        return builder.ThrowOnNull(true);
    }

    /// <summary>
    /// Provides a catch-all configuration object for the <see cref="IHttpBuilderConfig"/> for anything not handled by default
    /// </summary>
    /// <param name="builder">The builder to configure</param>
    /// <param name="config">The configuration value</param>
    /// <returns>The instance of <see cref="IHttpBuilderConfig"/> for chaining</returns>
    public static IHttpBuilderConfig With(this IHttpBuilderConfig builder, Action<IHttpBuilderConfig>? config)
    {
        config?.Invoke(builder);
        return builder;
    }

    /// <summary>
    /// Allows for specifying the <see cref="HttpClient"/> that will be used
    /// </summary>
    /// <param name="builder">The builder to configure</param>
    /// <param name="client">The instance of the <see cref="HttpClient"/></param>
    /// <returns>The instance of <see cref="IHttpBuilderConfig"/> for chaining</returns>
    /// <remarks>The client will be disposed after first use</remarks>
    public static IHttpBuilderConfig Client(this IHttpBuilderConfig builder, HttpClient client)
    {
        return builder.ClientFactory(_ => client);
    }

    /// <summary>
    /// Sets the method of the request
    /// </summary>
    /// <param name="builder">The builder to configure</param>
    /// <param name="method">The method to use</param>
    /// <returns>The instance of <see cref="IHttpBuilderConfig"/> for chaining</returns>
    public static IHttpBuilderConfig Method(this IHttpBuilderConfig builder, HttpMethod method)
    {
        return builder.Message(c => c.Method = method);
    }

    /// <summary>
    /// Sets the method of the request
    /// </summary>
    /// <param name="builder">The builder to configure</param>
    /// <param name="method">The method to use</param>
    /// <returns>The instance of <see cref="IHttpBuilderConfig"/> for chaining</returns>
    public static IHttpBuilderConfig Method(this IHttpBuilderConfig builder, string method)
    {
        return builder.Method(new HttpMethod(method.ToUpper().Trim()));
    }

    /// <summary>
    /// Sets the URI of the request
    /// </summary>
    /// <param name="builder">The builder to configure</param>
    /// <param name="uri">The URI to use</param>
    /// <returns>The instance of <see cref="IHttpBuilderConfig"/> for chaining</returns>
    public static IHttpBuilderConfig Uri(this IHttpBuilderConfig builder, Uri uri)
    {
        return builder.Message(c => c.RequestUri = uri);
    }

    /// <summary>
    /// Sets the URI of the request
    /// </summary>
    /// <param name="builder">The builder to configure</param>
    /// <param name="uri">The URI to use</param>
    /// <param name="parameters">Any query parameters to attach to the URI</param>
    /// <returns>The instance of <see cref="IHttpBuilderConfig"/> for chaining</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="uri"/> is null</exception>
    /// <exception cref="ArgumentException">Thrown when the <paramref name="uri"/> is not a valid URI</exception>
    public static IHttpBuilderConfig Uri(this IHttpBuilderConfig builder, string uri, params (string, string)[] parameters)
    {
        if (string.IsNullOrEmpty(uri))
            throw new ArgumentNullException(nameof(uri));

        foreach(var (k, v) in parameters)
            uri = uri.AppendQueryParam(k, v);

        if (!System.Uri.TryCreate(uri, UriKind.RelativeOrAbsolute, out var u))
            throw new ArgumentException("Invalid URI", nameof(uri));

        return builder.Uri(u);
    }

    /// <summary>
    /// Sets the URI of the request
    /// </summary>
    /// <param name="builder">The builder to configure</param>
    /// <param name="parts">The parts of the URI to build</param>
    /// <param name="parameters">Any query parameters to attach to the URI</param>
    /// <returns>The instance of <see cref="IHttpBuilderConfig"/> for chaining</returns>
    /// <exception cref="ArgumentNullException">Thrown when the built uri is null</exception>
    /// <exception cref="ArgumentException">Thrown when the built uri is not a valid URI</exception>
    public static IHttpBuilderConfig Uri(this IHttpBuilderConfig builder, string[] parts, params (string, string)[] parameters)
    {
        var uri = string.Join("/", parts.Select(t => t.Trim('/')));
        return builder.Uri(uri, parameters);
    }

    /// <summary>
    /// Sets the User-Agent header for the request
    /// </summary>
    /// <param name="builder">The builder to configure</param>
    /// <param name="userAgent">The User-Agent to use</param>
    /// <returns></returns>
    public static IHttpBuilderConfig UserAgent(this IHttpBuilderConfig builder, string userAgent)
    {
        return builder.Message(c => c.Headers.Add("User-Agent", userAgent));
    }

    /// <summary>
    /// Sets the timeout for the request
    /// </summary>
    /// <param name="builder">The builder to configure</param>
    /// <param name="timespan">How long to wait before timing out</param>
    /// <returns>The instance of <see cref="IHttpBuilderConfig"/> for chaining</returns>
    public static IHttpBuilderConfig Timeout(this IHttpBuilderConfig builder, TimeSpan timespan)
    {
        return builder.ClientConfig(c => c.Timeout = timespan);
    }

    /// <summary>
    /// Sets the timeout for the request
    /// </summary>
    /// <param name="builder">The builder to configure</param>
    /// <param name="seconds">How many seconds to wait before timing out</param>
    /// <returns>The instance of <see cref="IHttpBuilderConfig"/> for chaining</returns>
    public static IHttpBuilderConfig Timeout(this IHttpBuilderConfig builder, double seconds)
    {
        return builder.Timeout(TimeSpan.FromSeconds(seconds));
    }
}
