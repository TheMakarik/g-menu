namespace GMenu.JSONSerializationContexts;

[JsonSourceGenerationOptions(WriteIndented = true, PropertyNamingPolicy = StaticConfiguration.DefaultJsonNamingPolicy)]
[JsonSerializable(typeof(ObservableConfiguration))]
public sealed partial class ObservableConfigurationSerializerContext : JsonSerializerContext;