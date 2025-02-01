using System.Text.Json.Serialization;

namespace CardboardBox.Http.Tests.Shared;

public class FileUploadResult
{
    [JsonPropertyName("fileName")]
    public string? FileName { get; set; }

    [JsonPropertyName("bytes")]
    public long Bytes { get; set; }
}
