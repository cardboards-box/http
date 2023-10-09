namespace CardboardBox.Json.InterfaceDeserializing;

/// <summary>
/// Indicates how to deserialize an interface property on this type
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class InterfaceAttribute : Attribute
{
    /// <summary>
    /// The type of interface to search for
    /// </summary>
    public Type Interface { get; }

    /// <summary>
    /// The name of the property that indicates which type to use
    /// </summary>
    public string TypePropertyName { get; }

    /// <summary>
    /// The name of the property to put the payload into
    /// </summary>
    public string PayloadPropertyName { get; }

    /// <summary>
    /// The name of the property to put the raw JSON payload into
    /// </summary>
    public string? RawPayloadPropertyName { get; }

    /// <summary>
    /// Indicates how to deserialize an interface property on this type
    /// </summary>
    /// <param name="interface">The type of interface to search for</param>
    /// <param name="typePropertyName">The name of the property that indicates which type to use</param>
    /// <param name="payloadPropertyName">The name of the property to put the payload into</param>
    /// <param name="rawPayloadPropertyName">The name of the property to put the raw JSON payload into</param>
    public InterfaceAttribute(
        Type @interface,
        string typePropertyName,
        string payloadPropertyName,
        string? rawPayloadPropertyName = null)
    {
        Interface = @interface;
        PayloadPropertyName = payloadPropertyName;
        TypePropertyName = typePropertyName;
        RawPayloadPropertyName = rawPayloadPropertyName;
    }
}
