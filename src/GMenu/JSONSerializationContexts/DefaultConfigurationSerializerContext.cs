namespace GMenu.JSONSerializationContexts;

[JsonSourceGenerationOptions(WriteIndented = true, PropertyNamingPolicy = StaticConfiguration.DefaultJsonNamingPolicy, Converters = [typeof(CultureInfoJSONConverter)])]
[JsonSerializable(typeof(DefaultJSONConfiguration))]
public sealed  partial class DefaultConfigurationSerializerContext : JsonSerializerContext;