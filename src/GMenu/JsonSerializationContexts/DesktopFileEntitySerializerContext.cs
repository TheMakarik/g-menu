namespace GMenu.JsonSerializationContexts;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.Unspecified)]
[JsonSerializable(typeof(DesktopFile))]
public sealed partial class DesktopFileSerializerContext : JsonSerializerContext;