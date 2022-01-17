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
}