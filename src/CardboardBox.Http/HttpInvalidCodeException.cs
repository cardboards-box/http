namespace CardboardBox.Http;

/// <summary>
/// Indicates that the HTTP status code was not a success code
/// </summary>
/// <param name="body">The body of the response</param>
/// <param name="code">The status code of the response</param>
/// <param name="status">The status message of the response</param>
public class HttpInvalidCodeException(
    int code, string status, string body) : Exception($"HTTP Status Code Invalid: {code} - {status}")
{
    /// <summary>
    /// The status code of the response
    /// </summary>
    public int Code => code;

    /// <summary>
    /// The status message of the response
    /// </summary>
    public string Status => status;

    /// <summary>
    /// The body of the response
    /// </summary>
    public string Body => body;
}
