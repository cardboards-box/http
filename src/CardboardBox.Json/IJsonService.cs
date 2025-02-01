namespace CardboardBox.Json;

/// <summary>
/// Exposes common Json serialization and deserialization methods
/// </summary>
public interface IJsonService
{
    /// <summary>
    /// Deserializes the given string from JSON to the given type
    /// </summary>
    /// <typeparam name="T">The type to deserialize to</typeparam>
    /// <param name="data">The JSON string</param>
    /// <returns>The deserialized result</returns>
    T? Deserialize<T>(string data);

    /// <summary>
    /// Deserializes the given stream from JSON to the given type
    /// </summary>
    /// <typeparam name="T">The type to deserialize to</typeparam>
    /// <param name="stream">The stream of JSON data</param>
    /// <param name="token">The token to cancel the request</param>
    /// <returns>A task representing the deserialized result</returns>
    Task<T?> Deserialize<T>(Stream stream, CancellationToken token = default);

    /// <summary>
    /// Serializes the given data to JSON
    /// </summary>
    /// <typeparam name="T">The type of data to serialize</typeparam>
    /// <param name="data">The data to serialize</param>
    /// <returns>The serialized JSON</returns>
    string Serialize<T>(T data);

    /// <summary>
    /// Serializes the given data to JSON
    /// </summary>
    /// <typeparam name="T">The type of data to serialize</typeparam>
    /// <param name="data">The data to serialize</param>
    /// <param name="stream">The stream to write the serialized data to</param>
    /// <param name="token">The token to cancel the request</param>
    /// <returns>A task representing the completion of the serialization process</returns>
    Task Serialize<T>(T data, Stream stream, CancellationToken token = default);
}
