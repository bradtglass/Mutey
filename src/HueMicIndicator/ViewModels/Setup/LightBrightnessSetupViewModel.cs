namespace HueMicIndicator.ViewModels.Setup;

public class LightBrightnessSetupViewModel : LightFieldSetupViewModel, IAffectsColor
{
    private double brightness = 1;

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

    public (byte? a, (byte r, byte g, byte b)?) GetColorComponents()
        => (GetBrightness(), null);
}