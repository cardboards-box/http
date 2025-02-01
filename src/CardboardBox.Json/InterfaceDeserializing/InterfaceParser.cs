using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace CardboardBox.Json.InterfaceDeserializing;

/// <summary>
/// The implementation of the <see cref="JsonConverter"/> for interfaces
/// </summary>
/// <typeparam name="T">The type of class to deserialize</typeparam>
public class InterfaceParser<T> : JsonConverter<T>
{
    private static readonly InterfaceMap<T> _map = new();

    internal static void PopulateRegularProps(T instance, JsonNode parent, JsonSerializerOptions options)
    {
        foreach (var (json, prop, _, ignore) in _map.RegularProps)
        {
            if (ignore == JsonIgnoreCondition.Always) continue;

            var name = options.PropertyNamingPolicy?.ConvertName(json) ?? json;

            var node = parent[name];
            if (node == null) continue;

            var value = node.Deserialize(prop.PropertyType, options);
            prop.SetValue(instance, value);
        }
    }

    internal static object? GetDefault(Type type)
    {
        return type.IsValueType ? Activator.CreateInstance(type) : null;
    }

    /// <summary>
    /// Read and convert the JSON to T.
    /// </summary>
    /// <remarks>
    /// A converter may throw any Exception, but should throw <cref>JsonException</cref> when the JSON is invalid.
    /// </remarks>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read from.</param>
    /// <param name="typeToConvert">The <see cref="Type"/> being converted.</param>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> being used.</param>
    /// <returns>The value that was converted.</returns>
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var parent = JsonNode.Parse(ref reader);
        if (parent == null) return default;

        var instance = Activator.CreateInstance<T>();
        PopulateRegularProps(instance, parent, options);

        var type = parent[_map.TypeProp.JsonName]?.ToString();
        if (string.IsNullOrEmpty(type)) throw new JsonException($"{_map.TypeProp.JsonName} property is missing");

        _map.TypeProp.Property.SetValue(instance, type);

        var typeMap = _map.Map.FirstOrDefault(m => m.Name == type);

        var payload = parent[_map.PayloadProp.JsonName];
        _map.RawPayloadProp?.Property.SetValue(instance, payload?.ToString());

        if (typeMap != null)
        {
            var payloadInstance = payload?.Deserialize(typeMap.Map, options);
            _map.PayloadProp.Property.SetValue(instance, payloadInstance);
        }

        return instance;
    }

    /// <summary>
    /// Write the value as JSON.
    /// </summary>
    /// <remarks>
    /// A converter may throw any Exception, but should throw <cref>JsonException</cref> when the JSON
    /// cannot be created.
    /// </remarks>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write to.</param>
    /// <param name="value">The value to convert.</param>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> being used.</param>
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        var typeName = _map.TypeProp.Property.GetValue(value)?.ToString();
        var typeMap = _map.Map.FirstOrDefault(m => m.Name == typeName);
        writer.WriteStartObject();

        foreach (var (name, prop, type, ignore) in _map.Properties)
        {
            var val = prop.GetValue(value);
            if (val == null) continue;

            if (type == InterfacePropType.Payload)
            {
                if (typeMap == null) continue;

                writer.WritePropertyName(name);
                JsonSerializer.Serialize(writer, val, typeMap.Map, options);
                continue;
            }

            if (ignore == JsonIgnoreCondition.Always ||
                ignore == JsonIgnoreCondition.WhenWritingNull && val == null ||
                ignore == JsonIgnoreCondition.WhenWritingDefault && val.Equals(GetDefault(prop.PropertyType)))
                continue;

            writer.WritePropertyName(name);
            JsonSerializer.Serialize(writer, val, options);
        }

        writer.WriteEndObject();
    }
}