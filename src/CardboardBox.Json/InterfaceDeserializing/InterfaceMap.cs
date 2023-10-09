using System.Reflection;
using System.Text.Json.Serialization;

namespace CardboardBox.Json.InterfaceDeserializing;

/// <summary>
/// Represents the properties on an interface and the associated types
/// </summary>
/// <typeparam name="T">The type of class the properties exist on</typeparam>
public class InterfaceMap<T>
{
    private InterfaceProp[]? _props;
    private Type? _type;
    private InterfaceAttribute? _attr;
    private TypeMap[]? _map;

    /// <summary>
    /// All of the properties on the interface
    /// </summary>
    public InterfaceProp[] Properties => _props ??= GetProps().OrderBy(t => t.Type).ToArray();

    /// <summary>
    /// The type of class
    /// </summary>
    public Type Type => _type ??= typeof(T);

    /// <summary>
    /// The attribute that indicates the configuration of the interface
    /// </summary>
    public InterfaceAttribute Attribute => _attr ??= Type.GetCustomAttribute<InterfaceAttribute>()
        ?? throw new InvalidOperationException($"Type {Type} does not have {nameof(InterfaceAttribute)}");

    /// <summary>
    /// The property that indicates the field used to determine the type
    /// </summary>
    public InterfaceProp TypeProp => Properties.First(x => x.Type == InterfacePropType.Type);

    /// <summary>
    /// The property the payload is stored in
    /// </summary>
    public InterfaceProp PayloadProp => Properties.First(x => x.Type == InterfacePropType.Payload);

    /// <summary>
    /// The property the raw JSON payload should be stored in
    /// </summary>
    public InterfaceProp? RawPayloadProp => Properties.FirstOrDefault(x => x.Type == InterfacePropType.RawPayload);

    /// <summary>
    /// Everything that isn't the type or payload property
    /// </summary>
    public IEnumerable<InterfaceProp> RegularProps => Properties.Where(x => x.Type == InterfacePropType.Other);

    /// <summary>
    /// All of the types that implement this type
    /// </summary>
    public TypeMap[] Map => _map ??= GetMap().ToArray();

    private IEnumerable<InterfaceProp> GetProps()
    {
        if (Attribute.TypePropertyName == Attribute.PayloadPropertyName)
            throw new InvalidOperationException(
                $"{nameof(InterfaceAttribute.TypePropertyName)} and " +
                $"{nameof(InterfaceAttribute.PayloadPropertyName)} cannot be the same property");

        var props = Type.GetProperties();

        bool foundPayload = false,
             foundType = false;

        foreach (var prop in props)
        {
            var ignore = prop.GetCustomAttribute<JsonIgnoreAttribute>();
            var json = prop.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? prop.Name;
            var type = InterfacePropType.Other;

            if (prop.Name == Attribute.TypePropertyName)
            {
                type = InterfacePropType.Type;
                foundType = true;
            }
            else if (prop.Name == Attribute.PayloadPropertyName)
            {
                type = InterfacePropType.Payload;
                foundPayload = true;
            }
            else if (prop.Name == Attribute.RawPayloadPropertyName)
            {
                type = InterfacePropType.RawPayload;
                if (prop.PropertyType != typeof(string))
                    throw new InvalidOperationException($"Property {prop.Name} on type {Type} must be of type {typeof(string)}");
            }

            yield return new InterfaceProp(json, prop, type, ignore?.Condition ?? JsonIgnoreCondition.Never);
        }

        if (!foundPayload)
            throw new InvalidOperationException($"Type {Type} does not have a property named {Attribute.PayloadPropertyName}");
        if (!foundType)
            throw new InvalidOperationException($"Type {Type} does not have a property named {Attribute.TypePropertyName}");
    }

    private IEnumerable<TypeMap> GetMap()
    {
        var inf = Attribute.Interface;
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => x.IsClass && !x.IsAbstract && inf.IsAssignableFrom(x));
        foreach (var type in types)
        {
            var names = type.GetCustomAttributes<InterfaceOptionAttribute>()
                .SelectMany(x => x.Names)
                .Distinct();
            foreach (var attr in names)
            {
                yield return new TypeMap(attr, type);
            }
        }
    }
}
