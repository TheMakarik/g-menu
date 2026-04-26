namespace GMenu.JsonSerializationContexts;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.Unspecified, Converters = [typeof(CultureInfoJsonConverter)])]
[JsonSerializable(typeof(Dictionary<string, string>))]
public sealed partial class DictionarySerializerContext : JsonSerializerContext;