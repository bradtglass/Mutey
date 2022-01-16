using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Q42.HueApi.ColorConverters;

namespace HueMicIndicator.Hue;

public class RgbConverter : JsonConverter<RGBColor?>
{
    public override RGBColor? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => new(reader.GetString()!); // Serializer will not pass null to us

    public override void Write(Utf8JsonWriter writer, RGBColor? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
            writer.WriteStringValue(value.Value.ToHex());
        else
            writer.WriteNullValue();
    }
}