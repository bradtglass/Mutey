using HueMicIndicator.Hue.State.Color;
using Q42.HueApi;

namespace HueMicIndicator.Hue.State;

public record HueLightSetting(bool? On, byte? Brightness, HueColor? Color, bool ResetFirst) : IModifiesLightCommand
{
    public void Apply(LightCommand command)
    {
        Color?.Apply(command);
        command.On = On;
        command.Brightness = Brightness;
    }
}