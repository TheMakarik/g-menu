namespace GMenu.JsonSerializationContexts;

[JsonSourceGenerationOptions(WriteIndented = true, PropertyNamingPolicy = StaticConfiguration.DefaultJsonNamingPolicy, Converters = [typeof(CultureInfoJsonConverter)])]
[JsonSerializable(typeof(GMenuOptions))]
public sealed partial class GMenuOptionsSerializationContext : JsonSerializerContext;