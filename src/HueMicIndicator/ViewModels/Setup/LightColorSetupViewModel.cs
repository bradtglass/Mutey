using System.Windows.Media;

namespace HueMicIndicator.ViewModels.Setup;

public class LightColorSetupViewModel : LightFieldSetupViewModel
{
    private Color color;

    public LightColorSetupViewModel() : base(LightField.Color) { }

    public Color Color
    {
        get => color;
        set => SetProperty(ref color, value);
    }
}