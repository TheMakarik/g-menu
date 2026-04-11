namespace GMenu.JSONSerializationContexts;

[JsonSourceGenerationOptions(WriteIndented = true, PropertyNamingPolicy = StaticConfiguration.DefaultJsonNamingPolicy, Converters = [typeof(CultureInfoJSONConverter)])]
[JsonSerializable(typeof(ObservableConfiguration))]
public sealed partial class ObservableConfigurationSerializerContext : JsonSerializerContext;