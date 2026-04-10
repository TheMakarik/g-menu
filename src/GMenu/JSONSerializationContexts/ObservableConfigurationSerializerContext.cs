using System.Text.Json.Serialization;
using GMenu.Modules.Configuration.Model;

namespace GMenu.JSONSerializationContexts;

[JsonSourceGenerationOptions(WriteIndented = true, PropertyNamingPolicy = StaticConfiguration.DefaultJsonNamingPolicy)]
[JsonSerializable(typeof(ObservableConfiguration))]
public sealed partial class ObservableConfigurationSerializerContext : JsonSerializerContext;