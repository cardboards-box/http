using System.Text.Json.Serialization;

namespace CardboardBox.Http.Tests.Shared;

public class FailedResult
{
    [JsonPropertyName("code")]
    public required int Code { get; set; }

    [JsonPropertyName("message")]
    public required string Message { get; set; }
}
