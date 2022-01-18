using System.Windows.Media;
using HueMicIndicator.Hue.State.Color;

namespace HueMicIndicator.ViewModels.Setup;

public class LightColorSetupViewModel : LightColorSetupViewModelBase
{
    private Color color;

    public LightColorSetupViewModel() : base(LightField.Color) { }

    public Color Color
    {
        get => color;
        set => SetProperty(ref color, value);
    }

    public override HueColor GetHueColor()
        => new RgbHueColor(Color.ScR, Color.ScG, Color.ScB);
}