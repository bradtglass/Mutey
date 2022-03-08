using Q42.HueApi;

namespace HueMicIndicator.Hue.State.Color;

public interface IModifiesLightCommand
{
    void Apply(LightCommand command);
}