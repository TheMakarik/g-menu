namespace GMenu.JsonSerializationContexts;

[JsonSourceGenerationOptions(WriteIndented = true, PropertyNamingPolicy = StaticConfiguration.DefaultJsonNamingPolicy, Converters = [typeof(CultureInfoJsonConverter)])]
[JsonSerializable(typeof(ObservableConfiguration))]
public sealed partial class ObservableConfigurationSerializerContext : JsonSerializerContext;