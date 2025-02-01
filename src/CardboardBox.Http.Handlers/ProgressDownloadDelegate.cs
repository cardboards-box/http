namespace CardboardBox.Http.Handlers;

/// <summary>
/// The delegate used for tracking the progress of a download
/// </summary>
/// <param name="percentage">The percentage of the download</param>
/// <param name="bytes">The number of bytes downloaded</param>
/// <param name="elapsed">How long the download has been in progress</param>
public delegate void ProgressDownloadDelegate(int percentage, long bytes, TimeSpan elapsed);
