using System;
using HueMicIndicator.Hue.State;
using HueMicIndicator.Hue.State.Color;

namespace HueMicIndicator.Hue;

public static class Extensions
{
    public static HueLightSetting GetSetting(this Q42.HueApi.State state)
    {
        HueColor? color;

        // ReSharper disable once ConstantConditionalAccessQualifier
        switch (state.ColorMode?.ToLower())
        {
            case "xy":
                color = new XyHueColor(state.ColorCoordinates[0], state.ColorCoordinates[1]);
                break;
            case "ct":
                color = new TemperatureHueColor(1e6 / state.ColorTemperature.GetValueOrDefault(200));
                break;
            case null:
                Console.WriteLine($"Unknown color mode: {state.ColorMode}");
                color = null;
                break;
            default:
                color = null;
                break;
        }

        return new HueLightSetting(state.On, state.Brightness, color, false);
    }
}