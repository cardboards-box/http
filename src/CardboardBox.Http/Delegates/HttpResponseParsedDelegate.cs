namespace CardboardBox.Http.Delegates;

/// <summary>
/// Delegate for when a response is parsed from a request
/// </summary>
/// <param name="response">The response that was received</param>
/// <param name="parsed">The object that was parsed</param>
public delegate void HttpResponseParsedDelegate(HttpResponseMessage response, object? parsed);
