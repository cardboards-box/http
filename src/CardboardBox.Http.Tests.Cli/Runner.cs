using Microsoft.Extensions.Logging;

namespace CardboardBox.Http.Tests.Cli;

using Shared;
using Handlers;

internal class Runner(
    IApiService _api,
    ILogger<Runner> _logger)
{
    public static string ApiUrl => "https://localhost:7005/api/v1/test/";

    public async Task<int> Run()
    {
        try
        {
            _logger.LogInformation("Press any key to start tests");
            Console.ReadKey();

            var users = await _api.Get<UserAccount[]>(ApiUrl + "get");
            _logger.LogInformation("Users: {users}", users?.Length ?? 0);

            var failedUsers = await _api.Get<UserAccount[], FailedResult>(ApiUrl + "get?count=200");
            _logger.LogInformation("Failing Users: [{success}] {users} - {message}",
                failedUsers.Success, failedUsers.Result?.Length ?? 0, failedUsers.ErrorResult?.Message);

            var postAccount = await _api.Post<UserAccountResult, UserAccount>(ApiUrl + "post", new UserAccount
            {
                UserName = "Test",
                Password = "Password"
            });
            _logger.LogInformation("Post Account: {message}", postAccount?.Message ?? "Post was null");

            var putAccount = await _api.Put<UserAccountResult, UserAccount>(ApiUrl + "put?message=test", new UserAccount
            {
                UserName = "Test",
                Password = "Password"
            });
            _logger.LogInformation("Post Account: {message}", putAccount?.Message ?? "Put was null");

            var fileName = await ProgressTests();
            _logger.LogInformation("File uploaded: {file}", fileName ?? "No file uploaded");
            if (!string.IsNullOrEmpty(fileName))
                await DownloadTest(fileName);
            return 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while running tests");
            return 1;
        }
    }

    public async Task<string?> ProgressTests()
    {
        const string FILE_PATH = @"C:\Users\Cardboard\Downloads\large.zip";
        if (!File.Exists(FILE_PATH))
        {
            _logger.LogWarning("Could not find file to upload: {file} - Are you not Cardboard? Dad? Where'd you go?", FILE_PATH);
            return null;
        }

        using var io = File.OpenRead(FILE_PATH);
        using var stream = new MultipartFormDataContent();
        using var file = new StreamContent(io);
        stream.Add(file, "file", Path.GetFileName(FILE_PATH));

        var response = await _api.Post<FileUploadResult>(ApiUrl + "postFile", stream, c => c
            .OnStarting(() => _logger.LogInformation("Post file is starting"))
            .OnFinished(t => _logger.LogInformation("Post file is finished: {error}", t?.ToString() ?? "No error!"))
            .ProgressTracking(t => t
                //.OnDownload((percent, bytes, elapsed) => _logger.LogDebug("Download Progress - Immediate - {percent}% ({bytes}) - {elapsed}", percent, bytes, elapsed))
                //.OnUpload((percent, bytes, elapsed) => _logger.LogDebug("Upload Progress - Immediate - {percent}% ({bytes}) - {elapsed}", percent, bytes, elapsed))
                .OnDownloadTimer((percent, bytes, elapsed) => _logger.LogInformation("Download Progress - {percent}% ({bytes}) - {elapsed}", percent, bytes, elapsed))
                .OnUploadTimer((percent, bytes, elapsed) => _logger.LogInformation("Upload Progress - {percent}% ({bytes}) - {elapsed}", percent, bytes, elapsed))));

        _logger.LogInformation("File uploaded: {file} - {bytes}",
            response?.FileName ?? "Upload failed",
            response?.Bytes.ToString() ?? "No Bytes found");

        if (response is null) return null;

        return response.FileName;
    }

    public async Task DownloadTest(string filename)
    {
        var output = Path.GetTempFileName();
        using var io = File.Create(output);

        using var response = await _api.Get(ApiUrl + "downloadFile?fileName=" + filename, c => c
            .OnStarting(() => _logger.LogInformation("Download file is starting"))
            .OnFinished(t => _logger.LogInformation("Download file is finished: {error}", t?.ToString() ?? "No error!"))
            .ProgressTracking(t => t
                //.OnDownload((percent, bytes, elapsed) => _logger.LogDebug("Download Progress - Immediate - {percent}% ({bytes}) - {elapsed}", percent, bytes, elapsed))
                .OnDownloadTimer((percent, bytes, elapsed) => _logger.LogInformation("Download Progress - {percent}% ({bytes}) - {elapsed}", percent, bytes, elapsed))));
        if (response is null)
        {
            _logger.LogError("Download failed");
            return;
        }

        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync();
        await stream.CopyToAsync(io);
        _logger.LogInformation("Finished download!");
    }
}
