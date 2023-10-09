namespace CardboardBox.Json.InterfaceDeserializing;

/// <summary>
/// The type of interface property
/// </summary>
public enum InterfacePropType
{
    /// <summary>
    /// Regular property
    /// </summary>
    Other = 0,
    /// <summary>
    /// The property that tells what interface it's deserializing
    /// </summary>
    Type = 1,
    /// <summary>
    /// The property that contains the payload
    /// </summary>
    Payload = 2,
    /// <summary>
    /// The optional property to put the raw JSON payload in
    /// </summary>
    RawPayload = 3
}
