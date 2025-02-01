namespace CardboardBox.Http;

/// <summary>
/// Extension methods for <see cref="IApiService"/>
/// </summary>
public static class ApiServiceExtensions
{
    /// <summary>
    /// The verb used to GET something
    /// </summary>
    public const string VERB_GET = "GET";

    /// <summary>
    /// The verb used to DELETE something
    /// </summary>
    public const string VERB_DELETE = "DELETE";

    /// <summary>
    /// The verb used to POST something
    /// </summary>
    public const string VERB_POST = "POST";

    /// <summary>
    /// The verb used to POST something
    /// </summary>
    public const string VERB_PUT = "PUT";

    #region GET Requests
    /// <summary>
    /// Creates a GET request for the given URL
    /// </summary>
    /// <typeparam name="T">The return type</typeparam>
    /// <param name="api">The API service to use</param>
    /// <param name="url">The URL of the request</param>
    /// <param name="config">Any optional configuration necessary</param>
    /// <param name="token">The cancellation token for the request</param>
    /// <returns>A Task representing the results of the request</returns>
    public static Task<T?> Get<T>(this IApiService api, string url, Action<IHttpBuilderConfig>? config = null, CancellationToken? token = null)
    {
        return api.Create(url, VERB_GET, config, token).Result<T>();
    }

    /// <summary>
    /// Creates a GET request for the given URL
    /// </summary>
    /// <typeparam name="TSuccess">The return type for a successful request</typeparam>
    /// <typeparam name="TFailure">The return type for a failed request</typeparam>
    /// <param name="api">The API service to use</param>
    /// <param name="url">The URL of the request</param>
    /// <param name="config">Any optional configuration necessary</param>
    /// <param name="token">The cancellation token for the request</param>
    /// <returns>A Task representing the results of the request</returns>
    public static Task<HttpStatusResult<TSuccess, TFailure>> Get<TSuccess, TFailure>(this IApiService api, string url, Action<IHttpBuilderConfig>? config = null, CancellationToken? token = null)
    {
        return ((IHttpBuilder)api.Create(url, VERB_GET, config, token).FailGracefully()).Result<TSuccess, TFailure>();
    }
    #endregion

    #region POST requests
    /// <summary>
    /// Creates a POST request for the given URL and data
    /// </summary>
    /// <typeparam name="TResult">The return type</typeparam>
    /// <typeparam name="TData">The data type</typeparam>
    /// <param name="api">The API service to use</param>
    /// <param name="url">The URL of the request</param>
    /// <param name="data">The body of the POST request</param>
    /// <param name="config">Any optional configuration necessary</param>
    /// <param name="token">The cancellation token for the request</param>
    /// <returns>A task representing the results of the request</returns>
    public static Task<TResult?> Post<TResult, TData>(this IApiService api, string url, TData data, Action<IHttpBuilderConfig>? config = null, CancellationToken? token = null)
    {
        return ((IHttpBuilder)api.Create(url, VERB_POST, config, token).Body(data)).Result<TResult>();
    }

    /// <summary>
    /// Creates a POST request for the given URL and data
    /// </summary>
    /// <typeparam name="TSuccess">The return type for a successful request</typeparam>
    /// <typeparam name="TFailure">he return type for a failed request</typeparam>
    /// <typeparam name="TData">The data type</typeparam>
    /// <param name="api">The API service to use</param>
    /// <param name="url">The URL of the request</param>
    /// <param name="data">The body of the POST request</param>
    /// <param name="config">Any optional configuration necessary</param>
    /// <param name="token">The cancellation token for the request</param>
    /// <returns>A task representing the results of the request</returns>
    public static Task<HttpStatusResult<TSuccess, TFailure>> Post<TSuccess, TFailure, TData>(this IApiService api, string url, TData data, Action<IHttpBuilderConfig>? config = null, CancellationToken? token = null)
    {
        return ((IHttpBuilder)api.Create(url, VERB_POST, config, token).Body(data).FailGracefully()).Result<TSuccess, TFailure>();
    }

    /// <summary>
    /// Creates a POST request for the given URL and data.
    /// Data will be encoded using <see cref="FormUrlEncodedContent"/>
    /// </summary>
    /// <typeparam name="T">The return type</typeparam>
    /// <param name="api">The API service to use</param>
    /// <param name="url">The URL of the request</param>
    /// <param name="formData">The body of the POST request</param>
    /// <param name="config">Any optional configuration necessary</param>
    /// <param name="token">The cancellation token for the request</param>
    /// <returns>A task representing the results of the request</returns>
    public static Task<T?> Post<T>(this IApiService api, string url, (string key, string val)[] formData, Action<IHttpBuilderConfig>? config = null, CancellationToken? token = null)
    {
        return ((IHttpBuilder)api.Create(url, VERB_POST, config, token).Body(formData)).Result<T>();
    }

    /// <summary>
    /// Creates a POST request for the given URL and data.
    /// Data will be encoded using <see cref="FormUrlEncodedContent"/>
    /// </summary>
    /// <typeparam name="TSuccess">The return type for a successful request</typeparam>
    /// <typeparam name="TFailure">he return type for a failed request</typeparam>
    /// <param name="api">The API service to use</param>
    /// <param name="url">The URL of the request</param>
    /// <param name="data">The body of the POST request</param>
    /// <param name="config">Any optional configuration necessary</param>
    /// <param name="token">The cancellation token for the request</param>
    /// <returns>A task representing the results of the request</returns>
    public static Task<HttpStatusResult<TSuccess, TFailure>> Post<TSuccess, TFailure>(this IApiService api, string url, (string key, string val)[] data, Action<IHttpBuilderConfig>? config = null, CancellationToken? token = null)
    {
        return ((IHttpBuilder)api.Create(url, VERB_POST, config, token).Body(data).FailGracefully()).Result<TSuccess, TFailure>();
    }

    /// <summary>
    /// Creates a POST request for the given URL and data.
    /// </summary>
    /// <typeparam name="T">The return type</typeparam>
    /// <param name="api">The API service to use</param>
    /// <param name="url">The URL of the request</param>
    /// <param name="content">The body of the request</param>
    /// <param name="config">Any optional configuration necessary</param>
    /// <param name="token">The cancellation token for the request</param>
    /// <returns>A task representing the results of the request</returns>
    public static Task<T?> Post<T>(this IApiService api, string url, HttpContent content, Action<IHttpBuilderConfig>? config = null, CancellationToken? token = null)
    {
        return ((IHttpBuilder)api.Create(url, VERB_POST, config, token).BodyContent(content)).Result<T>();
    }

    /// <summary>
    /// Creates a POST request for the given URL and data.
    /// </summary>
    /// <typeparam name="TSuccess">The return type for a successful request</typeparam>
    /// <typeparam name="TFailure">he return type for a failed request</typeparam>
    /// <param name="api">The API service to use</param>
    /// <param name="url">The URL of the request</param>
    /// <param name="content">The body of the request</param>
    /// <param name="config">Any optional configuration necessary</param>
    /// <param name="token">The cancellation token for the request</param>
    /// <returns>A task representing the results of the request</returns>
    public static Task<HttpStatusResult<TSuccess, TFailure>> Post<TSuccess, TFailure>(this IApiService api, string url, HttpContent content, Action<IHttpBuilderConfig>? config = null, CancellationToken? token = null)
    {
        return ((IHttpBuilder)api.Create(url, VERB_POST, config, token).BodyContent(content).FailGracefully()).Result<TSuccess, TFailure>();
    }
    #endregion

    #region PUT requests
    /// <summary>
    /// Creates a PUT request for the given URL and data
    /// </summary>
    /// <typeparam name="TResult">The return type</typeparam>
    /// <typeparam name="TData">The data type</typeparam>
    /// <param name="api">The API service to use</param>
    /// <param name="url">The URL of the request</param>
    /// <param name="data">The body of the PUT request</param>
    /// <param name="config">Any optional configuration necessary</param>
    /// <param name="token">The cancellation token for the request</param>
    /// <returns>A task representing the results of the request</returns>
    public static Task<TResult?> Put<TResult, TData>(this IApiService api, string url, TData data, Action<IHttpBuilderConfig>? config = null, CancellationToken? token = null)
    {
        return ((IHttpBuilder)api.Create(url, VERB_PUT, config, token).Body(data)).Result<TResult>();
    }

    /// <summary>
    /// Creates a PUT request for the given URL and data
    /// </summary>
    /// <typeparam name="TSuccess">The return type for a successful request</typeparam>
    /// <typeparam name="TFailure">he return type for a failed request</typeparam>
    /// <typeparam name="TData">The data type</typeparam>
    /// <param name="api">The API service to use</param>
    /// <param name="url">The URL of the request</param>
    /// <param name="data">The body of the PUT request</param>
    /// <param name="config">Any optional configuration necessary</param>
    /// <param name="token">The cancellation token for the request</param>
    /// <returns>A task representing the results of the request</returns>
    public static Task<HttpStatusResult<TSuccess, TFailure>> Put<TSuccess, TFailure, TData>(this IApiService api, string url, TData data, Action<IHttpBuilderConfig>? config = null, CancellationToken? token = null)
    {
        return ((IHttpBuilder)api.Create(url, VERB_PUT, config, token).Body(data).FailGracefully()).Result<TSuccess, TFailure>();
    }

    /// <summary>
    /// Creates a PUT request for the given URL and data.
    /// Data will be encoded using <see cref="FormUrlEncodedContent"/>
    /// </summary>
    /// <typeparam name="T">The return type</typeparam>
    /// <param name="api">The API service to use</param>
    /// <param name="url">The URL of the request</param>
    /// <param name="formData">The body of the POST request</param>
    /// <param name="config">Any optional configuration necessary</param>
    /// <param name="token">The cancellation token for the request</param>
    /// <returns>A task representing the results of the request</returns>
    public static Task<T?> Put<T>(this IApiService api, string url, (string key, string val)[] formData, Action<IHttpBuilderConfig>? config = null, CancellationToken? token = null)
    {
        return ((IHttpBuilder)api.Create(url, VERB_PUT, config, token).Body(formData)).Result<T>();
    }

    /// <summary>
    /// Creates a PUT request for the given URL and data.
    /// Data will be encoded using <see cref="FormUrlEncodedContent"/>
    /// </summary>
    /// <typeparam name="TSuccess">The return type for a successful request</typeparam>
    /// <typeparam name="TFailure">he return type for a failed request</typeparam>
    /// <param name="api">The API service to use</param>
    /// <param name="url">The URL of the request</param>
    /// <param name="data">The body of the PUT request</param>
    /// <param name="config">Any optional configuration necessary</param>
    /// <param name="token">The cancellation token for the request</param>
    /// <returns>A task representing the results of the request</returns>
    public static Task<HttpStatusResult<TSuccess, TFailure>> Put<TSuccess, TFailure>(this IApiService api, string url, (string key, string val)[] data, Action<IHttpBuilderConfig>? config = null, CancellationToken? token = null)
    {
        return ((IHttpBuilder)api.Create(url, VERB_PUT, config, token).Body(data).FailGracefully()).Result<TSuccess, TFailure>();
    }

    /// <summary>
    /// Creates a PUT request for the given URL and data.
    /// </summary>
    /// <typeparam name="T">The return type</typeparam>
    /// <param name="api">The API service to use</param>
    /// <param name="url">The URL of the request</param>
    /// <param name="content">The body of the request</param>
    /// <param name="config">Any optional configuration necessary</param>
    /// <param name="token">The cancellation token for the request</param>
    /// <returns>A task representing the results of the request</returns>
    public static Task<T?> Put<T>(this IApiService api, string url, HttpContent content, Action<IHttpBuilderConfig>? config = null, CancellationToken? token = null)
    {
        return ((IHttpBuilder)api.Create(url, VERB_PUT, config, token).BodyContent(content)).Result<T>();
    }

    /// <summary>
    /// Creates a PUT request for the given URL and data.
    /// </summary>
    /// <typeparam name="TSuccess">The return type for a successful request</typeparam>
    /// <typeparam name="TFailure">he return type for a failed request</typeparam>
    /// <param name="api">The API service to use</param>
    /// <param name="url">The URL of the request</param>
    /// <param name="content">The body of the request</param>
    /// <param name="config">Any optional configuration necessary</param>
    /// <param name="token">The cancellation token for the request</param>
    /// <returns>A task representing the results of the request</returns>
    public static Task<HttpStatusResult<TSuccess, TFailure>> Put<TSuccess, TFailure>(this IApiService api, string url, HttpContent content, Action<IHttpBuilderConfig>? config = null, CancellationToken? token = null)
    {
        return ((IHttpBuilder)api.Create(url, VERB_PUT, config, token).BodyContent(content).FailGracefully()).Result<TSuccess, TFailure>();
    }
    #endregion

    #region DELETE requests
    /// <summary>
    /// Creates a DELETE request for the given URL
    /// </summary>
    /// <typeparam name="T">The return type</typeparam>
    /// <param name="api">The API service to use</param>
    /// <param name="url">The URL of the request</param>
    /// <param name="config">Any optional configuration necessary</param>
    /// <param name="token">The cancellation token for the request</param>
    /// <returns>A Task representing the results of the request</returns>
    public static Task<T?> Delete<T>(this IApiService api, string url, Action<IHttpBuilderConfig>? config = null, CancellationToken? token = null)
    {
        return ((IHttpBuilder)api.Create(url, VERB_DELETE, config, token)).Result<T>();
    }

    /// <summary>
    /// Creates a DELETE request for the given URL
    /// </summary>
    /// <typeparam name="TSuccess">The return type for a successful request</typeparam>
    /// <typeparam name="TFailure">The return type for a failed request</typeparam>
    /// <param name="api">The API service to use</param>
    /// <param name="url">The URL of the request</param>
    /// <param name="config">Any optional configuration necessary</param>
    /// <param name="token">The cancellation token for the request</param>
    /// <returns>A Task representing the results of the request</returns>
    public static Task<HttpStatusResult<TSuccess, TFailure>> Delete<TSuccess, TFailure>(this IApiService api, string url, Action<IHttpBuilderConfig>? config = null, CancellationToken? token = null)
    {
        return ((IHttpBuilder)api.Create(url, VERB_DELETE, config, token).FailGracefully()).Result<TSuccess, TFailure>();
    }

    /// <summary>
    /// Creates a DELETE request for the given URL and data
    /// </summary>
    /// <typeparam name="TResult">The return type</typeparam>
    /// <typeparam name="TData">The data type</typeparam>
    /// <param name="api">The API service to use</param>
    /// <param name="url">The URL of the request</param>
    /// <param name="data">The body of the DELETE request</param>
    /// <param name="config">Any optional configuration necessary</param>
    /// <param name="token">The cancellation token for the request</param>
    /// <returns>A task representing the results of the request</returns>
    public static Task<TResult?> Delete<TResult, TData>(this IApiService api, string url, TData data, Action<IHttpBuilderConfig>? config = null, CancellationToken? token = null)
    {
        return ((IHttpBuilder)api.Create(url, VERB_DELETE, config, token).Body(data)).Result<TResult>();
    }

    /// <summary>
    /// Creates a DELETE request for the given URL and data
    /// </summary>
    /// <typeparam name="TSuccess">The return type for a successful request</typeparam>
    /// <typeparam name="TFailure">he return type for a failed request</typeparam>
    /// <typeparam name="TData">The data type</typeparam>
    /// <param name="api">The API service to use</param>
    /// <param name="url">The URL of the request</param>
    /// <param name="data">The body of the DELETE request</param>
    /// <param name="config">Any optional configuration necessary</param>
    /// <param name="token">The cancellation token for the request</param>
    /// <returns>A task representing the results of the request</returns>
    public static Task<HttpStatusResult<TSuccess, TFailure>> Delete<TSuccess, TFailure, TData>(this IApiService api, string url, TData data, Action<IHttpBuilderConfig>? config = null, CancellationToken? token = null)
    {
        return ((IHttpBuilder)api.Create(url, VERB_DELETE, config, token).Body(data).FailGracefully()).Result<TSuccess, TFailure>();
    }

    /// <summary>
    /// Creates a DELETE request for the given URL and data.
    /// Data will be encoded using <see cref="FormUrlEncodedContent"/>
    /// </summary>
    /// <typeparam name="T">The return type</typeparam>
    /// <param name="api">The API service to use</param>
    /// <param name="url">The URL of the request</param>
    /// <param name="formData">The body of the DELETE request</param>
    /// <param name="config">Any optional configuration necessary</param>
    /// <param name="token">The cancellation token for the request</param>
    /// <returns>A task representing the results of the request</returns>
    public static Task<T?> Delete<T>(this IApiService api, string url, (string key, string val)[] formData, Action<IHttpBuilderConfig>? config = null, CancellationToken? token = null)
    {
        return ((IHttpBuilder)api.Create(url, VERB_DELETE, config, token).Body(formData)).Result<T>();
    }

    /// <summary>
    /// Creates a DELETE request for the given URL and data.
    /// Data will be encoded using <see cref="FormUrlEncodedContent"/>
    /// </summary>
    /// <typeparam name="TSuccess">The return type for a successful request</typeparam>
    /// <typeparam name="TFailure">he return type for a failed request</typeparam>
    /// <param name="api">The API service to use</param>
    /// <param name="url">The URL of the request</param>
    /// <param name="data">The body of the DELETE request</param>
    /// <param name="config">Any optional configuration necessary</param>
    /// <param name="token">The cancellation token for the request</param>
    /// <returns>A task representing the results of the request</returns>
    public static Task<HttpStatusResult<TSuccess, TFailure>> Delete<TSuccess, TFailure>(this IApiService api, string url, (string key, string val)[] data, Action<IHttpBuilderConfig>? config = null, CancellationToken? token = null)
    {
        return ((IHttpBuilder)api.Create(url, VERB_DELETE, config, token).Body(data).FailGracefully()).Result<TSuccess, TFailure>();
    }

    /// <summary>
    /// Creates a DELETE request for the given URL and data.
    /// </summary>
    /// <typeparam name="T">The return type</typeparam>
    /// <param name="api">The API service to use</param>
    /// <param name="url">The URL of the request</param>
    /// <param name="content">The body of the request</param>
    /// <param name="config">Any optional configuration necessary</param>
    /// <param name="token">The cancellation token for the request</param>
    /// <returns>A task representing the results of the request</returns>
    public static Task<T?> Delete<T>(this IApiService api, string url, HttpContent content, Action<IHttpBuilderConfig>? config = null, CancellationToken? token = null)
    {
        return ((IHttpBuilder)api.Create(url, VERB_DELETE, config, token).BodyContent(content)).Result<T>();
    }

    /// <summary>
    /// Creates a DELETE request for the given URL and data.
    /// </summary>
    /// <typeparam name="TSuccess">The return type for a successful request</typeparam>
    /// <typeparam name="TFailure">he return type for a failed request</typeparam>
    /// <param name="api">The API service to use</param>
    /// <param name="url">The URL of the request</param>
    /// <param name="content">The body of the request</param>
    /// <param name="config">Any optional configuration necessary</param>
    /// <param name="token">The cancellation token for the request</param>
    /// <returns>A task representing the results of the request</returns>
    public static Task<HttpStatusResult<TSuccess, TFailure>> Delete<TSuccess, TFailure>(this IApiService api, string url, HttpContent content, Action<IHttpBuilderConfig>? config = null, CancellationToken? token = null)
    {
        return ((IHttpBuilder)api.Create(url, VERB_DELETE, config, token).BodyContent(content).FailGracefully()).Result<TSuccess, TFailure>();
    }
    #endregion
}
