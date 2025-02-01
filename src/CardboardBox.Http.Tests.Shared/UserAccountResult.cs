using System.Text.Json.Serialization;

namespace CardboardBox.Http.Tests.Shared;

public class UserAccountResult
{
    [JsonPropertyName("message")]
    public required string Message { get; set; }

    [JsonPropertyName("user")]
    public required UserAccount User { get; set; }
}
