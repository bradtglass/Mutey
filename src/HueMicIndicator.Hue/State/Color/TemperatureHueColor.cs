using System.Text.Json.Serialization;
using Q42.HueApi;

namespace HueMicIndicator.Hue.State.Color;

[JsonConverter(typeof(HueColorConverter))]
public class TemperatureHueColor : HueColor
{
    public TemperatureHueColor(double temperature)
    {
        Temperature = temperature;
    }

    /// <summary>
    ///     The temperature, in Kelvin.
    /// </summary>
    public double Temperature { get; }

    public override void Apply(LightCommand command)
        => command.ColorTemperature = (int?)(1e6 / Temperature);
}