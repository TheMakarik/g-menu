namespace GMenu.JsonSerializationContexts;

[JsonSourceGenerationOptions(WriteIndented = true, Converters = [typeof(CultureInfoJsonConverter)])]
[JsonSerializable(typeof(GMenuOptions))]
public sealed partial class GMenuOptionsSerializationContext : JsonSerializerContext;