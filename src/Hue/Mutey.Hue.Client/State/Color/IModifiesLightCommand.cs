using Q42.HueApi;

namespace Mutey.Hue.Client.State.Color;

public interface IModifiesLightCommand
{
    void Apply(LightCommand command);
}