namespace HueMicIndicator.ViewModels.Setup;

public class LightColorTempSetupViewModel : LightFieldSetupViewModel
{
    private double temperature;
    
    public LightColorTempSetupViewModel() : base(LightField.ColorTemperature) { }

    public double Temperature
    {
        get => temperature;
        set => SetProperty(ref temperature, value);
    }
}