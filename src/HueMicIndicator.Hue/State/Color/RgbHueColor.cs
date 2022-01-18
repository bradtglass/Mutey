using System.Text.Json.Serialization;
using Q42.HueApi;
using Q42.HueApi.ColorConverters;
using Q42.HueApi.ColorConverters.HSB;

namespace HueMicIndicator.Hue.State.Color;

[JsonConverter(typeof(HueColorConverter))]
public sealed class RgbHueColor : HueColor
{
    public RgbHueColor(double r, double g, double b) : this(new RGBColor(r, g, b)) { }

    public RgbHueColor(RGBColor color)
    {
        Color = color;
    }

    public RGBColor Color { get; }

    public override void Apply(LightCommand command)
        => command.SetColor(Color);

    private bool Equals(RgbHueColor other)
        => Color.Equals(other.Color);

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((RgbHueColor)obj);
    }

    public override int GetHashCode()
        => Color.GetHashCode();
}