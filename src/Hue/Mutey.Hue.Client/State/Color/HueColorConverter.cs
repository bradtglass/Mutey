using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Q42.HueApi.ColorConverters;

namespace Mutey.Hue.Client.State.Color;

public class HueColorConverter : JsonConverter<HueColor>
{
    public override bool CanConvert(Type typeToConvert)
        => typeToConvert.IsAssignableTo(typeof(HueColor));

    public override HueColor? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TokenType switch
        {
            JsonTokenType.StartObject => ReadXyColor(ref reader),
            JsonTokenType.String => ParseColor(reader.GetString()!),
            JsonTokenType.Null => null,
            _ => throw new ArgumentOutOfRangeException(nameof(reader), reader.TokenType, "Invalid token type")
        };

    private static HueColor ParseColor(string value)
    {
        if (value[0] == '#')
            return new RgbHueColor(new RGBColor(value[1..]));

        if (value[^1] == 'K' || value[^1] == 'k')
            return new TemperatureHueColor(double.Parse(value[..^2]));

        throw new FormatException($"Invalid colour format: {value}");
    }

    private static HueColor ReadXyColor(ref Utf8JsonReader reader)
    {
        // Read the start object
        reader.Read();
        var x = ReadNumeric("X", ref reader);
        var y = ReadNumeric("Y", ref reader);

        return new XyHueColor(x, y);

        double ReadNumeric(string name, ref Utf8JsonReader innerReader)
        {
            if (innerReader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException("Expected a property name");

            var readName = innerReader.GetString()!;
            if (!string.Equals(readName, name, StringComparison.OrdinalIgnoreCase))
                throw new JsonException($"Expected '{name}', actual '{readName}'");

            // Read past property name
            innerReader.Read();

            if (innerReader.TokenType != JsonTokenType.Number)
                throw new JsonException("Expected a numeric value");

            var number = innerReader.GetDouble();
            innerReader.Read();

            return number;
        }
    }

    public override void Write(Utf8JsonWriter writer, HueColor value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case null:
                writer.WriteNullValue();
                break;
            case RgbHueColor rgbHueColor:
                WriteRgb(writer, rgbHueColor);
                break;
            case TemperatureHueColor temperatureHueColor:
                WriteTemp(writer, temperatureHueColor);
                break;
            case XyHueColor xyHueColor:
                WriteXy(writer, xyHueColor);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(value), value, "Unhandled color type");
        }
    }

    private static void WriteRgb(Utf8JsonWriter writer, RgbHueColor color)
        => writer.WriteStringValue($"#{color.Color.ToHex()}");

    private static void WriteTemp(Utf8JsonWriter writer, TemperatureHueColor color)
        => writer.WriteStringValue($"{color.Temperature}K");

    private static void WriteXy(Utf8JsonWriter writer, XyHueColor color)
    {
        writer.WriteStartObject();
        writer.WriteNumber("X", color.X);
        writer.WriteNumber("Y", color.Y);
        writer.WriteEndObject();
    }
}