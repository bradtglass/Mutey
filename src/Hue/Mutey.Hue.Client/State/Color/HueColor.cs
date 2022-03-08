using System.Text.Json.Serialization;
using Q42.HueApi;

namespace Mutey.Hue.Client.State.Color;

[JsonConverter(typeof(HueColorConverter))]
public abstract class HueColor : IModifiesLightCommand
{
    public abstract void Apply(LightCommand command);
}