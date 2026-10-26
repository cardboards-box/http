namespace CardboardBox.Http;

using Delegates;

/// <summary>
/// Providers a builder used for configuration HTTP requests from the given <see cref="IHttpClientFactory"/>
/// </summary>
public interface IHttpBuilder
{
    /// <summary>
    /// Triggered whenever a request finishes
    /// </summary>
    event HttpFinishedDelegate Finished;

    /// <summary>
    /// Triggers whenever a request is starting
    /// </summary>
    event HttpStartingDelegate Starting;

    /// <summary>
    /// Triggers whenever a response is received 
    /// </summary>
    /// <remarks>
    /// You should avoid processing the body of the response in reaction to this delegate, 
    /// otherwise you can mess with the way the library processes the response streams and 
    /// cause issues with the deserialization of the response.
    /// </remarks>
    event HttpResponseReceivedDelegate ResponseReceived;

    /// <summary>
    /// Triggers when a response is parsed from a request
    /// </summary>
    /// <remarks>This only works for built in JSON responses</remarks>
    event HttpResponseParsedDelegate ResponseParsed;

    /// <summary>
    /// The json service to use for serialization
    /// </summary>
    IJsonService JsonService { get; }

    /// <summary>
    /// The client factory
    /// </summary>
    IHttpClientFactory Factory { get; }

    /// <summary>
    /// Provides a catch-all configuration object for the <see cref="HttpRequestMessage"/> for anything not handled by default
    /// </summary>
    /// <param name="config">The configuration value</param>
    /// <returns>The instance of <see cref="IHttpBuilder"/> for chaining</returns>
    IHttpBuilder Message(Action<HttpRequestMessage>? config);

    /// <summary>
    /// Throws an exception if the result is null
    /// </summary>
    /// <param name="throwOnNull">Whether or not to throw an exception if the result is null</param>
    /// <returns>The instance of <see cref="IHttpBuilder"/> for chaining</returns>
    IHttpBuilder ThrowOnNull(bool throwOnNull = true);

    /// <summary>
    /// Adds a custom factory for creating the <see cref="HttpClient"/>
    /// </summary>
    /// <param name="factory">The factory for creating the client</param>
    /// <returns>The instance of <see cref="IHttpBuilder"/> for chaining</returns>
    IHttpBuilder ClientFactory(Func<IHttpClientFactory, HttpClient> factory);

    /// <summary>
    /// Configures the <see cref="HttpClient"/> for the request
    /// </summary>
    /// <param name="config">The configuration value</param>
    /// <returns>The instance of <see cref="IHttpBuilder"/> for chaining</returns>
    IHttpBuilder ClientConfig(Action<HttpClient>? config);

    /// <summary>
    /// Register a cancellation token to use for the request
    /// </summary>
    /// <param name="token">The token to cancel with</param>
    /// <returns>The instance of <see cref="IHttpBuilder"/> for chaining</returns>
    IHttpBuilder CancelWith(CancellationToken? token);

	/// <summary>
	/// Executes the HTTP request and returns the results as the given type
	/// </summary>
	/// <typeparam name="T">The type to deserialize the results to</typeparam>
	/// <returns>A task representing the results of the request</returns>
	Task<T?> Result<T>();

	/// <summary>
	/// Executes the HTTP request and returns the results as the given types
	/// </summary>
	/// <typeparam name="TSuccess">The type to use for a successful request</typeparam>
	/// <typeparam name="TFailure">The type to use for a failed request</typeparam>
	/// <returns>A task representing the <see cref="HttpStatusResult{TSuccess, TFailure}"/> which contains the results of the request </returns>
	Task<HttpStatusResult<TSuccess, TFailure>> Result<TSuccess, TFailure>();

	/// <summary>
	/// Executes the HTTP request and returns the results
	/// </summary>
	/// <returns>The <see cref="HttpResponseMessage"/> results</returns>
	/// <exception cref="ArgumentNullException">Thrown if the URI is not set for the request</exception>
	Task<HttpResponseMessage?> Result();
}

/// <inheritdoc cref="IHttpBuilder" />
[Obsolete("This interface is no longer necessary and will be removed in a future version. Use IHttpBuilder directly instead.")]
public interface IHttpBuilderConfig : IHttpBuilder { }

/// <summary>
/// Concrete implementation of <see cref="IHttpBuilder"/>
/// </summary>
/// <param name="_factory">The factory to use to build <see cref="HttpClient"/></param>
/// <param name="_json">The service to use for JSON parsing</param>
public class HttpBuilder(
    IHttpClientFactory _factory,
    IJsonService _json) : IHttpBuilder
{
    private readonly List<Action<HttpRequestMessage>> _messageEdits = [];
    private readonly List<Action<HttpClient>> _clientEdits = [];
    private Func<IHttpClientFactory, HttpClient>? _clientFactory;
    private CancellationTokenSource? _cancelSource;
    private readonly List<CancellationToken> _cancellationTokens = [];
    private readonly List<CancellationTokenRegistration> _cancellationRegistrations = [];

    private bool _failWithNull = false;

    /// <summary>
    /// The json service to use for serialization
    /// </summary>
    public virtual IJsonService JsonService => _json;

    /// <summary>
    /// The client factory
    /// </summary>
    public virtual IHttpClientFactory Factory => _factory;

    /// <summary>
    /// Triggered whenever a request finishes
    /// </summary>
    public event HttpFinishedDelegate Finished = delegate { };

    /// <summary>
    /// Triggers whenever a request is starting
    /// </summary>
    public event HttpStartingDelegate Starting = delegate { };

    /// <summary>
    /// Triggers whenever a response is received 
    /// </summary>
    /// <remarks>
    /// You should avoid processing the body of the response in reaction to this delegate, 
    /// otherwise you can mess with the way the library processes the response streams and 
    /// cause issues with the deserialization of the response.
    /// </remarks>
    public event HttpResponseReceivedDelegate ResponseReceived = delegate { };

    /// <summary>
    /// Triggers when a response is parsed from a request
    /// </summary>
    /// <remarks>This only works for built in JSON responses</remarks>
    public event HttpResponseParsedDelegate ResponseParsed = delegate { };

    #region Configuration Methods
    /// <summary>
    /// Throws an exception if the result is null
    /// </summary>
    /// <param name="throwOnNull">Whether or not to throw an exception if the result is null</param>
    /// <returns>The instance of <see cref="IHttpBuilder"/> for chaining</returns>
    public virtual IHttpBuilder ThrowOnNull(bool throwOnNull = true)
    {
        _failWithNull = !throwOnNull;
        return this;
    }

    /// <summary>
    /// Provides a catch-all configuration object for the <see cref="HttpRequestMessage"/> for anything not handled by default
    /// </summary>
    /// <param name="config">The configuration value</param>
    /// <returns>The instance of <see cref="IHttpBuilder"/> for chaining</returns>
    public virtual IHttpBuilder Message(Action<HttpRequestMessage>? config)
    {
        if (config != null)
            _messageEdits.Add(config);
        return this;
    }

    /// <summary>
    /// Configures the <see cref="HttpClient"/> for the request
    /// </summary>
    /// <param name="config">The configuration value</param>
    /// <returns>The instance of <see cref="IHttpBuilder"/> for chaining</returns>
    public virtual IHttpBuilder ClientConfig(Action<HttpClient>? config)
    {
        if (config != null)
            _clientEdits.Add(config);
        return this;
    }

    /// <summary>
    /// Adds a custom factory for creating the <see cref="HttpClient"/>
    /// </summary>
    /// <param name="factory">The factory for creating the client</param>
    /// <returns>The instance of <see cref="IHttpBuilder"/> for chaining</returns>
    public virtual IHttpBuilder ClientFactory(Func<IHttpClientFactory, HttpClient> factory)
    {
        _clientFactory = factory;
        return this;
    }

    /// <summary>
    /// Register a cancellation token to use for the request
    /// </summary>
    /// <param name="token">The token to cancel with</param>
    /// <returns>The instance of <see cref="IHttpBuilder"/> for chaining</returns>
    public virtual IHttpBuilder CancelWith(CancellationToken? token)
    {
        if (token is { CanBeCanceled: true } cancellation)
        {
            _cancellationTokens.Add(cancellation);
            if (_cancelSource is not null)
                _cancellationRegistrations.Add(cancellation.Register(static state =>
                    ((CancellationTokenSource)state!).Cancel(), _cancelSource));
        }
        return this;
    }
    #endregion

    #region Output Methods
    /// <summary>
    /// Executes the HTTP request and returns the results
    /// </summary>
    /// <returns>The <see cref="HttpResponseMessage"/> results</returns>
    /// <exception cref="ArgumentNullException">Thrown if the URI is not set for the request</exception>
    public virtual async Task<HttpResponseMessage?> Result()
    {
        try
        {
            var token = CreateCancellationToken();
            TriggerStarted();
            using var client = CreateHttpClient();
            var resp = await MakeRequest(client, false, token);
            TriggerFinished(null);
            return resp;
        }
        catch (Exception ex)
        {
            TriggerFinished(ex);
            if (_failWithNull) return null;

            throw;
        }
        finally { ReleaseCancellation(); }
    }

    /// <summary>
    /// Executes the HTTP request and returns the results as the given type
    /// </summary>
    /// <typeparam name="T">The type to deserialize the results to</typeparam>
    /// <returns>A task representing the results of the request</returns>
    /// <exception cref="ArgumentNullException">Thrown if the URI is not set for the request</exception>
    public virtual async Task<T?> Result<T>()
    {
        try
        {
            var token = CreateCancellationToken();
            TriggerStarted();
            using var client = CreateHttpClient();
            using var resp = await MakeRequest(client, true, token);
            var data = await Json<T>(resp, token);
            TriggerFinished(null);
            return data;
        }
        catch (Exception ex)
        {
            TriggerFinished(ex);
            if (_failWithNull) return default;

            throw;
        }
        finally { ReleaseCancellation(); }
    }

    /// <summary>
    /// Executes the HTTP request and returns the results as the given types
    /// </summary>
    /// <typeparam name="TSuccess">The type to use for a successful request</typeparam>
    /// <typeparam name="TFailure">The type to use for a failed request</typeparam>
    /// <returns>A task representing the <see cref="HttpStatusResult{TSuccess, TFailure}"/> which contains the results of the request </returns>
    public virtual async Task<HttpStatusResult<TSuccess, TFailure>> Result<TSuccess, TFailure>()
    {
        try
        {
            var token = CreateCancellationToken();
            TriggerStarted();
            using var client = CreateHttpClient();
            using var resp = await MakeRequest(client, true, token);
            var data = await Json<TSuccess, TFailure>(resp, token);
            TriggerFinished(null);
            return data;
        }
        catch (Exception ex)
        {
            TriggerFinished(ex);
            if (_failWithNull) 
                return HttpStatusResult<TSuccess, TFailure>.FromFailure(ex, HttpStatusCode.InternalServerError);

            throw;
        }
        finally { ReleaseCancellation(); }
    }
    #endregion

    #region Helper Methods
    /// <summary>
    /// Create the <see cref="HttpClient"/> to use for the request
    /// </summary>
    /// <returns>The <see cref="HttpClient"/> for the request</returns>
    public virtual HttpClient CreateHttpClient()
    {
        var client = _clientFactory is not null
            ? _clientFactory(_factory)
            : _factory.CreateClient();

        foreach(var config in _clientEdits)
            config?.Invoke(client);

        return client;
    }

    /// <summary>
    /// Create the cancellation token to use for the request
    /// </summary>
    /// <returns>The cancellation token</returns>
    public virtual CancellationToken CreateCancellationToken()
    {
        _cancelSource ??= _cancellationTokens.Count == 0
            ? new CancellationTokenSource()
            : CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokens.ToArray());
        return _cancelSource.Token;
    }

    private void ReleaseCancellation()
    {
        foreach (var registration in _cancellationRegistrations) registration.Dispose();
        _cancellationRegistrations.Clear();
        _cancelSource?.Dispose();
        _cancelSource = null;
    }

    /// <summary>
    /// Executes the given request and returns the <see cref="HttpRequestMessage"/>
    /// </summary>
    /// <param name="client">The <see cref="HttpClient"/> to use to make the request</param>
    /// <param name="ensureAccept">Whether or not to ensure the `application/json` accept header is present</param>
    /// <param name="token">The token to cancel the request</param>
    /// <returns>A task representing the results for the request</returns>
    public virtual async Task<HttpResponseMessage> MakeRequest(HttpClient client, bool ensureAccept, CancellationToken token)
    {
        using var request = new HttpRequestMessage();

        foreach (var config in _messageEdits)
            config?.Invoke(request);

        if (ensureAccept && request.Headers.Accept.Count == 0)
            request.Headers.Accept.ParseAdd(ApiServiceExtensions.MEDIA_TYPE_JSON);

        var response = await client.SendAsync(request, token);
        TriggerResponseReceived(response, request);
        return response;
    }

    /// <summary>
    /// Deserializes the <see cref="HttpResponseMessage"/> from Json to the given type
    /// </summary>
    /// <typeparam name="T">The type to deserialize too</typeparam>
    /// <param name="resp">The response message to deserialize</param>
    /// <param name="token">The token to cancel the request</param>
    /// <returns>A task representing the returned deserialized result</returns>
    public virtual async Task<T?> Json<T>(HttpResponseMessage resp, CancellationToken token)
    {
        if (!_failWithNull && !resp.IsSuccessStatusCode)
        {
#if NET10_0_OR_GREATER
            var content = await resp.Content.ReadAsStringAsync(token);
#else
            var content = await resp.Content.ReadAsStringAsync();
#endif
            TriggerResponseParsed(resp, content);
            throw new HttpInvalidCodeException((int)resp.StatusCode, resp.ReasonPhrase, content);
        }

#if NET10_0_OR_GREATER
        using var rs = await resp.Content.ReadAsStreamAsync(token);
#else
		using var rs = await resp.Content.ReadAsStreamAsync();
#endif
		var result = await _json.Deserialize<T>(rs, token);
        TriggerResponseParsed(resp, result);
        return result;
    }

    /// <summary>
    /// Deserializes the <see cref="HttpResponseMessage"/> from JSON to the given types
    /// </summary>
    /// <typeparam name="TSuccess">The type to use if the result was successful</typeparam>
    /// <typeparam name="TFailure">The type to use if the result was failure</typeparam>
    /// <param name="resp">The response message to deserialize</param>
    /// <param name="token">The token to cancel the request</param>
    /// <returns>A task representing the return deserialized result</returns>
    public virtual async Task<HttpStatusResult<TSuccess, TFailure>> Json<TSuccess, TFailure>(HttpResponseMessage resp, CancellationToken token)
	{
#if NET10_0_OR_GREATER
        using var rs = await resp.Content.ReadAsStreamAsync(token);
#else
		using var rs = await resp.Content.ReadAsStreamAsync();
#endif

		if (resp.IsSuccessStatusCode)
        {
            var data = await _json.Deserialize<TSuccess>(rs, token);
            TriggerResponseParsed(resp, data);
            return HttpStatusResult<TSuccess, TFailure>.FromSuccess(data, resp.StatusCode);
        }

        var error = await _json.Deserialize<TFailure>(rs, token);
        TriggerResponseParsed(resp, error);
        return HttpStatusResult<TSuccess, TFailure>.FromFailure(error, resp.StatusCode);
    }

    /// <summary>
    /// Triggers the finished actions
    /// </summary>
    /// <param name="ex">The exception that occurred</param>
    public virtual void TriggerFinished(Exception? ex)
    {
        Finished(ex);
    }

    /// <summary>
    /// Triggers the started actions
    /// </summary>
    public virtual void TriggerStarted()
    {
        Starting();
    }

    /// <summary>
    /// Triggers the response received actions
    /// </summary>
    /// <param name="response">The response that was received</param>
    /// <param name="request">The request that was sent</param>
    public virtual void TriggerResponseReceived(HttpResponseMessage response, HttpRequestMessage request)
    {
        ResponseReceived(response, request);
    }

    /// <summary>
    /// Triggers the response parsed actions
    /// </summary>
    /// <param name="response">The response that was received</param>
    /// <param name="parsed">The object that was parsed</param>
    public virtual void TriggerResponseParsed(HttpResponseMessage response, object? parsed)
    {
        ResponseParsed(response, parsed);
    }
#endregion
}
