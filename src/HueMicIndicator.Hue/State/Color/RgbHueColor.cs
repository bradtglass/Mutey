using System.Text.Json.Serialization;
using Q42.HueApi;
using Q42.HueApi.ColorConverters;
using Q42.HueApi.ColorConverters.HSB;

namespace HueMicIndicator.Hue.State.Color;

[JsonConverter(typeof(HueColorConverter))]
public class RgbHueColor : HueColor
{
    public RgbHueColor(RGBColor color)
    {
        Color = color;
    }

    public RGBColor Color { get; }

    public override void Apply(LightCommand command)
        => command.SetColor(Color);
}