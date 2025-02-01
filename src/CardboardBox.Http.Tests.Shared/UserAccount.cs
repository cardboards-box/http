using System.Text.Json.Serialization;

namespace CardboardBox.Http.Tests.Shared;

public class UserAccount
{
    [JsonPropertyName("userName")]
    public required string UserName { get; set; }

    [JsonPropertyName("password")]
    public required string Password { get; set; }
}
