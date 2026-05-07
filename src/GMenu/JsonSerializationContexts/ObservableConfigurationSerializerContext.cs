namespace GMenu.JsonSerializationContexts;

[JsonSourceGenerationOptions(
    WriteIndented = true,
    PropertyNamingPolicy = StaticConfiguration.DefaultJsonNamingPolicy,
    Converters = [typeof(CultureInfoJsonConverter), typeof(VersionJsonConverter)])]
[JsonSerializable(typeof(ObservableConfiguration))]
public sealed partial class ObservableConfigurationSerializerContext : JsonSerializerContext;