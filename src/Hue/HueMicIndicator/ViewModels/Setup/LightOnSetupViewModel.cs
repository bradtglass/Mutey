namespace HueMicIndicator.ViewModels.Setup;

public class LightOnSetupViewModel : LightFieldSetupViewModel
{
    private bool on;

    public LightOnSetupViewModel() : base(LightField.On) { }

    public bool On
    {
        get => on;
        set => SetProperty(ref on, value);
    }
}