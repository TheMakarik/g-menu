namespace GMenu.JSONSerializationContexts;

[JsonSourceGenerationOptions(WriteIndented = true, PropertyNamingPolicy = StaticConfiguration.DefaultJsonNamingPolicy)]
[JsonSerializable(typeof(DefaultJSONConfiguration))]
public sealed  partial class DefaultConfigurationSerializerContext : JsonSerializerContext;