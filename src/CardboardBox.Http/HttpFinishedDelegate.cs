namespace CardboardBox.Http;

/// <summary>
/// The delegate used for handling completion of an HTTP request
/// </summary>
/// <param name="exception">The optional exception that occurred</param>
public delegate void HttpFinishedDelegate(Exception? exception);
