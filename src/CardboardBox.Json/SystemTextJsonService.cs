using System.Text.Json;

namespace CardboardBox.Json;


/// <summary>
/// A concrete implementation of <see cref="IJsonService"/> that uses <see cref="JsonSerializer"/>
/// </summary>
public class SystemTextJsonService : IJsonService
{
    private readonly JsonSerializerOptions _settings;

    /// <summary>
    /// A concrete implementation of <see cref="IJsonService"/> that uses <see cref="JsonSerializer"/>
    /// </summary>
    /// <param name="settings">The json serialization options</param>
    public SystemTextJsonService(JsonSerializerOptions settings)
    {
        _settings = settings;
    }

    /// <summary>
    /// Deserializes the given string from JSON to the given type
    /// </summary>
    /// <typeparam name="T">The type to deserialize to</typeparam>
    /// <param name="data">The JSON string</param>
    /// <returns>The deserialized result</returns>
    public T? Deserialize<T>(string data) => JsonSerializer.Deserialize<T>(data, _settings);

    /// <summary>
    /// Deserializes the given stream from JSON to the given type
    /// </summary>
    /// <typeparam name="T">The type to deserialize to</typeparam>
    /// <param name="stream">The stream of JSON data</param>
    /// <param name="token">The cancellation token for the request</param>
    /// <returns>A task representing the deserialized result</returns>
    public async Task<T?> Deserialize<T>(Stream stream, CancellationToken token = default) => await JsonSerializer.DeserializeAsync<T>(stream, _settings, token);

    /// <summary>
    /// Serializes the given data to JSON
    /// </summary>
    /// <typeparam name="T">The type of data to serialize</typeparam>
    /// <param name="data">The data to serialize</param>
    /// <returns>The serialized JSON</returns>
    public string Serialize<T>(T data) => JsonSerializer.Serialize(data, _settings);

    /// <summary>
    /// Serializes the given data to JSON
    /// </summary>
    /// <typeparam name="T">The type of data to serialize</typeparam>
    /// <param name="data">The data to serialize</param>
    /// <param name="stream">The stream to write the serialized data to</param>
    /// <param name="token">The cancellation token for the request</param>
    /// <returns>A task representing the completion of the serialization process</returns>
    public Task Serialize<T>(T data, Stream stream, CancellationToken token = default) => JsonSerializer.SerializeAsync(stream, data, _settings, token);
}
