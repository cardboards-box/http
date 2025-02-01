namespace CardboardBox.Http.Handlers;

/// <summary>
/// A set of extensions for configuring <see cref="IHttpBuilder"/>s
/// </summary>
public static class HttpExtensions
{
    /// <summary>
    /// Add progress tracking to the HTTP builder
    /// </summary>
    /// <param name="builder">The <see cref="IHttpBuilderConfig"/> to attach to</param>
    /// <param name="config">The configuration action</param>
    /// <returns>The instance of <see cref="IHttpBuilderConfig"/> for chaining</returns>
    public static IHttpBuilderConfig ProgressTracking(this IHttpBuilderConfig builder, Action<IHttpProgressBuilderConfig> config)
    {
        var bob = new ProgressHttpBuilder(builder);
        config(bob);
        bob.Register();
        return builder;
    }

    /// <summary>
    /// How long to wait between reporting progress
    /// </summary>
    /// <param name="builder">The builder to attach to</param>
    /// <param name="seconds">The number of seconds to wait between reporting</param>
    /// <returns>The current <see cref="IHttpProgressBuilderConfig"/> instance for method chaining</returns>
    public static IHttpProgressBuilderConfig ReportIncrement(this IHttpProgressBuilderConfig builder, int seconds)
    {
        return builder.ReportIncrement(TimeSpan.FromSeconds(seconds));
    }

    /// <summary>
    /// Adds a delegate for tracking file download progress
    /// </summary>
    /// <param name="builder">The builder to attach to</param>
    /// <param name="download">The delegate to attach</param>
    /// <returns>The current <see cref="IHttpProgressBuilderConfig"/> instance for method chaining</returns>
    public static IHttpProgressBuilderConfig OnDownload(this IHttpProgressBuilderConfig builder, ProgressDownloadDelegate download)
    {
        builder.Download += download;
        return builder;
    }

    /// <summary>
    /// Adds a delegate for tracking file download progress - on a timer
    /// </summary>
    /// <param name="builder">The builder to attach to</param>
    /// <param name="download">The delegate to attach</param>
    /// <returns>The current <see cref="IHttpProgressBuilderConfig"/> instance for method chaining</returns>
    public static IHttpProgressBuilderConfig OnDownloadTimer(this IHttpProgressBuilderConfig builder, ProgressDownloadDelegate download)
    {
        builder.DownloadTimer += download;
        return builder;
    }

    /// <summary>
    /// Adds a delegate for tracking file upload progress
    /// </summary>
    /// <param name="builder">The builder to attach to</param>
    /// <param name="upload">The delegate to attach</param>
    /// <returns>The current <see cref="IHttpProgressBuilderConfig"/> instance for method chaining</returns>
    public static IHttpProgressBuilderConfig OnUpload(this IHttpProgressBuilderConfig builder, ProgressUploadDelegate upload)
    {
        builder.Upload += upload;
        return builder;
    }

    /// <summary>
    /// Adds a delegate for tracking file upload progress - on a timer
    /// </summary>
    /// <param name="builder">The builder to attach to</param>
    /// <param name="upload">The delegate to attach</param>
    /// <returns>The current <see cref="IHttpProgressBuilderConfig"/> instance for method chaining</returns>
    public static IHttpProgressBuilderConfig OnUploadTimer(this IHttpProgressBuilderConfig builder, ProgressUploadDelegate upload)
    {
        builder.UploadTimer += upload;
        return builder;
    }
}
