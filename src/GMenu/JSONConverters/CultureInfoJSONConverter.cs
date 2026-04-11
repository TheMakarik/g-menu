namespace GMenu.JSONConverters;

public class CultureInfoJSONConverter : JsonConverter<CultureInfo>
{
    public override CultureInfo? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var cultureName = reader.GetString();
        return string.IsNullOrEmpty(cultureName) ? null : CultureInfo.GetCultureInfo(cultureName);
    }

    public override void Write(Utf8JsonWriter writer, CultureInfo value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Name);
    }
}