namespace CardboardBox.Http.Handlers;

/// <summary>
/// A set of extensions for configuring <see cref="IHttpBuilder"/>s
/// </summary>
public static class HttpExtensions
{
    /// <summary>
    /// Add progress tracking to the HTTP builder
    /// </summary>
    /// <param name="config">The <see cref="IHttpBuilderConfig"/> to attach to</param>
    /// <returns>The instance of the <see cref="IHttpProgressBuilderConfig"/></returns>
    /// <remarks>Remember to call <see cref="IHttpProgressBuilderConfig.Finish"/></remarks>
    public static IHttpProgressBuilderConfig ProgressTracking(this IHttpBuilderConfig config)
    {
        return new ProgressHttpBuilder(config);
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
}
