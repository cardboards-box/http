namespace CardboardBox.Json.InterfaceDeserializing;

/// <summary>
/// Represents a type and it's associated name
/// </summary>
/// <param name="Name">The name of the type</param>
/// <param name="Map">The system type for the map</param>
public record class TypeMap(string Name, Type Map);
