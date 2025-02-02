namespace CardboardBox.Http.Delegates;

/// <summary>
/// Delegate for when a response is received from a request
/// </summary>
/// <param name="response">The response that was received</param>
/// <param name="request">The request that was sent</param>
/// <remarks>This triggers before the body is processed</remarks>
public delegate void HttpResponseReceivedDelegate(HttpResponseMessage response, HttpRequestMessage request);
