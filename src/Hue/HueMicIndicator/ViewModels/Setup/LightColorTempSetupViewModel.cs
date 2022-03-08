using HueMicIndicator.Hue.State.Color;

namespace HueMicIndicator.ViewModels.Setup;

public class LightColorTempSetupViewModel : LightColorSetupViewModelBase
{
    private double temperature = 2500;
    
    public LightColorTempSetupViewModel() : base(LightField.ColorTemperature) { }

    public double Temperature
    {
        get => temperature;
        set => SetProperty(ref temperature, value);
    }

    public override HueColor GetHueColor()
        => new TemperatureHueColor(Temperature);

    protected override (byte r, byte g, byte b) GetRgb()
    {
        var color = ColorUtility.TempToColor(Temperature);

        return (color.R, color.G, color.B);
    }
}