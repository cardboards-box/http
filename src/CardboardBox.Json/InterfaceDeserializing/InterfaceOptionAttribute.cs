namespace CardboardBox.Json.InterfaceDeserializing;

/// <summary>
/// Indicates that the class can be deserialized to an interface
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class InterfaceOptionAttribute : Attribute
{
    /// <summary>
    /// The value of the type property that indicates this type
    /// </summary>
    public string[] Names { get; }

    /// <summary>
    /// Indicates that the class can be deserialized to an interface
    /// </summary>
    /// <param name="names">The value of the type property that indicates this type</param>
    public InterfaceOptionAttribute(params string[] names)
    {
        Names = names;
    }
}
