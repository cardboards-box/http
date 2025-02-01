using System.Diagnostics;
using System.Net.Http.Handlers;

namespace CardboardBox.Http.Handlers;

/// <summary>
/// Providers a set of configuration extensions for adding progress tracking to <see cref="IHttpBuilderConfig"/>
/// </summary>
public interface IHttpProgressBuilderConfig
{
    /// <summary>
    /// Triggered when <see cref="ProgressMessageHandler.HttpReceiveProgress"/> is triggered
    /// </summary>
    event ProgressDownloadDelegate Download;

    /// <summary>
    /// Triggered on a timer and sends the most recent download progress
    /// </summary>
    event ProgressDownloadDelegate DownloadTimer;

    /// <summary>
    /// Triggered when <see cref="ProgressMessageHandler.HttpSendProgress"/> is triggered
    /// </summary>
    event ProgressUploadDelegate Upload;

    /// <summary>
    /// Triggered on a timer and sends the most recent upload progress
    /// </summary>
    event ProgressUploadDelegate UploadTimer;

    /// <summary>
    /// The factory for creating <see cref="HttpClientHandler"/>
    /// </summary>
    /// <param name="factory">The client handler</param>
    /// <returns>The current <see cref="IHttpProgressBuilderConfig"/> instance for method chaining</returns>
    IHttpProgressBuilderConfig HandlerFactory(Func<HttpClientHandler> factory);

    /// <summary>
    /// How long to wait between reporting progress
    /// </summary>
    /// <param name="increment">The increment to report</param>
    /// <returns>The current <see cref="IHttpProgressBuilderConfig"/> instance for method chaining</returns>
    IHttpProgressBuilderConfig ReportIncrement(TimeSpan increment);
}

/// <summary>
/// The implementation of <see cref="IHttpProgressBuilderConfig"/>
/// </summary>
/// <param name="_builder">The original <see cref="IHttpBuilderConfig"/></param>
public class ProgressHttpBuilder(
    IHttpBuilderConfig _builder) : IHttpProgressBuilderConfig
{
    private readonly List<IDisposable> _disposers = [];

    private long? _downloadBytes = null;
    private int? _downloadPercentage = null;
    private readonly Stopwatch _downloadTimer = new();

    private long? _uploadBytes = null;
    private int? _uploadPercentage = null;
    private readonly Stopwatch _uploadTimer = new();

    private Func<HttpClientHandler>? _handlerFactory = null;
    private CancellationTokenSource? _cancelSource = null;
    private TimeSpan _reportIncrement = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Triggered when <see cref="ProgressMessageHandler.HttpReceiveProgress"/> is triggered
    /// </summary>
    public event ProgressDownloadDelegate Download = delegate { };

    /// <summary>
    /// Triggered on a timer and sends the most recent download progress
    /// </summary>
    public event ProgressDownloadDelegate DownloadTimer = delegate { };

    /// <summary>
    /// Triggered when <see cref="ProgressMessageHandler.HttpSendProgress"/> is triggered
    /// </summary>
    public event ProgressUploadDelegate Upload = delegate { };

    /// <summary>
    /// Triggered on a timer and sends the most recent upload progress
    /// </summary>
    public event ProgressUploadDelegate UploadTimer = delegate { };

    /// <summary>
    /// The factory for creating <see cref="HttpClientHandler"/>
    /// </summary>
    /// <param name="factory">The client handler</param>
    /// <returns>The current <see cref="IHttpProgressBuilderConfig"/> instance for method chaining</returns>
    public IHttpProgressBuilderConfig HandlerFactory(Func<HttpClientHandler> factory)
    {
        _handlerFactory = factory;
        return this;
    }

    /// <summary>
    /// How long to wait between reporting progress
    /// </summary>
    /// <param name="increment">The increment to report</param>
    /// <returns>The current <see cref="IHttpProgressBuilderConfig"/> instance for method chaining</returns>
    public IHttpProgressBuilderConfig ReportIncrement(TimeSpan increment)
    {
        _reportIncrement = increment;
        return this;
    }

    /// <summary>
    /// Goes back to the original configuration builder
    /// </summary>
    /// <returns>The configuration builder this progress tracker is attached to</returns>
    /// <remarks>Make sure this is called</remarks>
    internal IHttpProgressBuilderConfig Register()
    {
        _builder.Starting += BuilderOnStarting;
        return this;
    }

    internal HttpClientHandler GetHandler()
    {
        return _handlerFactory?.Invoke() ?? new HttpClientHandler();
    }

    internal async Task ReportProgress(CancellationToken token)
    {
        //Ensure we box and void the "TaskCancelledException" so it doesn't cause issues later
        try
        {
            //Keep looping until cancellation is requested
            while (!token.IsCancellationRequested)
            {
                //Wait for the interval to pass before reporting the progress
                await Task.Delay(_reportIncrement, token);

                bool hasDownload = false, hasUpload = false;
                //If we have download progress, report it
                if (_downloadBytes.HasValue && _downloadPercentage.HasValue)
                {
                    hasDownload = true;
                    DownloadTimer(_downloadPercentage.Value, _downloadBytes.Value, _downloadTimer.Elapsed);
                    //If the progress is 100%, reset the values so we don't keep reporting it
                    if (_downloadPercentage.Value >= 100)
                    {
                        _downloadBytes = null;
                        _downloadPercentage = null;
                        _downloadTimer.Stop();
                    }
                }

                //If we have upload progress, report it
                if (_uploadBytes.HasValue && _uploadPercentage.HasValue)
                {
                    hasUpload = true;
                    UploadTimer(_uploadPercentage.Value, _uploadBytes.Value, _uploadTimer.Elapsed);
                    //If the progress is 100%, reset the values so we don't keep reporting it
                    if (_uploadPercentage.Value >= 100)
                    {
                        _uploadBytes = null;
                        _uploadPercentage = null;
                        _uploadTimer.Stop();
                    }
                }

                //We only want to continue past this point if we have finished reporting progress
                if (!hasDownload || !hasUpload || 
                    _uploadBytes.HasValue || _uploadPercentage.HasValue ||
                    _downloadBytes.HasValue || _downloadPercentage.HasValue) continue;

                break;
            }
        }
        catch (TaskCanceledException) { }
    }

    internal void BuilderOnStarting()
    {
        _builder.Finished += BuilderOnFinished;

        var httpHandler = GetHandler();
        var handler = new ProgressMessageHandler(httpHandler);
        var client = new HttpClient(handler);
        _cancelSource = new CancellationTokenSource();

        _builder.Client(client);

        _disposers.AddRange([httpHandler, handler, _cancelSource]);

        handler.HttpReceiveProgress += (_, e) =>
        {
            if (!_downloadBytes.HasValue)
                _downloadTimer.Start();

            _downloadBytes = e.BytesTransferred;
            _downloadPercentage = e.ProgressPercentage;
            Download(e.ProgressPercentage, e.BytesTransferred, _downloadTimer.Elapsed);
        };

        handler.HttpSendProgress += (_, e) =>
        {
            if (!_uploadBytes.HasValue)
                _uploadTimer.Start();

            _uploadBytes = e.BytesTransferred;
            _uploadPercentage = e.ProgressPercentage;
            Upload(e.ProgressPercentage, e.BytesTransferred, _uploadTimer.Elapsed);
        };

        _ = Task.Run(() => ReportProgress(_cancelSource.Token), _cancelSource.Token);
    }

    internal void BuilderOnFinished(Exception? exception)
    {
        _cancelSource?.Cancel();
        foreach (var disposer in _disposers)
        {
            try
            {
                disposer.Dispose();
            }
            catch { }
        }
        _cancelSource = null;
        _disposers.Clear();
        _builder.Finished -= BuilderOnFinished;
    }
}
