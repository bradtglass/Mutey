using System.Text.Json.Serialization;
using Q42.HueApi;

namespace HueMicIndicator.Hue.State.Color;

[JsonConverter(typeof(HueColorConverter))]
public class XyHueColor : HueColor
{
    public XyHueColor(double x, double y)
    {
        X = x;
        Y = y;
    }

    public double X { get; }

    public double Y { get; }

    public override void Apply(LightCommand command)
        => command.SetColor(X, Y);
}