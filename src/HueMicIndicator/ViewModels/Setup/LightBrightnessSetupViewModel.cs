namespace HueMicIndicator.ViewModels.Setup;

public class LightBrightnessSetupViewModel : LightFieldSetupViewModel
{
    private double brightness;

    public LightBrightnessSetupViewModel() : base(LightField.Brightness) { }

    public double Brightness
    {
        get => brightness;
        set => SetProperty(ref brightness, value);
    }

    public void SetBrightness(byte value)
        => Brightness = (double)value / byte.MaxValue;

    public byte GetBrightness()
        => (byte)(Brightness * byte.MaxValue);
}