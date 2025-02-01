namespace CardboardBox.Http.Handlers;

/// <summary>
/// The delegate used for tracking the progress of an upload
/// </summary>
/// <param name="percentage">The percentage of the upload</param>
/// <param name="bytes">The number of bytes uploaded</param>
/// <param name="elapsed">How long the upload has been in progress</param>
public delegate void ProgressUploadDelegate(int percentage, long bytes, TimeSpan elapsed);
