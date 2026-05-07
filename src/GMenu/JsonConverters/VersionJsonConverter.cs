namespace GMenu.JsonConverters;

public sealed class VersionJsonConverter : JsonConverter<Version>
{
    public override Version? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return string.IsNullOrEmpty(value) ? null : new Version(value);
    }

    public override void Write(Utf8JsonWriter writer, Version value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}