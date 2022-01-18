using System;
using System.Text.Json.Serialization;
using Q42.HueApi;

namespace HueMicIndicator.Hue.State.Color;

[JsonConverter(typeof(HueColorConverter))]
public sealed class XyHueColor : HueColor
{
    public XyHueColor(double x, double y)
    {
        X = x;
        Y = y;
    }

    public double X { get; }

    public double Y { get; }

    private bool Equals(XyHueColor other)
        => X.Equals(other.X) && Y.Equals(other.Y);

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((XyHueColor)obj);
    }

    public override int GetHashCode()
        => HashCode.Combine(X, Y);

    public override void Apply(LightCommand command)
        => command.SetColor(X, Y);
}