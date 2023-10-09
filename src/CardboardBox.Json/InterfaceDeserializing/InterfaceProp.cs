using System.Reflection;
using System.Text.Json.Serialization;

namespace CardboardBox.Json.InterfaceDeserializing;

/// <summary>
/// Represents a property on an interface
/// </summary>
/// <param name="JsonName">The value of the <see cref="JsonPropertyNameAttribute"/></param>
/// <param name="Property">The property that was fetched</param>
/// <param name="Type">The type of interface property</param>
/// <param name="Ignore">The value of the <see cref="JsonIgnoreAttribute"/></param>
public record class InterfaceProp(
    string JsonName,
    PropertyInfo Property,
    InterfacePropType Type,
    JsonIgnoreCondition Ignore);
