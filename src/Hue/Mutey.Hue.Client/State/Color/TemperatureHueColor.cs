using System.Text.Json.Serialization;
using Q42.HueApi;

namespace Mutey.Hue.Client.State.Color;

[JsonConverter(typeof(HueColorConverter))]
public sealed class TemperatureHueColor : HueColor
{
    public TemperatureHueColor(double temperature)
    {
        Temperature = temperature;
    }

    /// <summary>
    ///     The temperature, in Kelvin.
    /// </summary>
    public double Temperature { get; }

    private bool Equals(TemperatureHueColor other)
        => Temperature.Equals(other.Temperature);

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((TemperatureHueColor)obj);
    }

    public override int GetHashCode()
        => Temperature.GetHashCode();

    public override void Apply(LightCommand command)
        => command.ColorTemperature = (int?)(1e6 / Temperature);
}