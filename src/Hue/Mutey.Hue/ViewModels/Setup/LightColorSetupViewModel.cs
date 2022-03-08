using System.Windows.Media;
using Mutey.Hue.Client.State.Color;

namespace Mutey.Hue.ViewModels.Setup;

public class LightColorSetupViewModel : LightColorSetupViewModelBase
{
    private Color color = Colors.MediumBlue;

    public LightColorSetupViewModel() : base(LightField.Color) { }

    public Color Color
    {
        get => color;
        set => SetProperty(ref color, value);
    }

    public override HueColor GetHueColor()
        => new RgbHueColor(Color.ScR, Color.ScG, Color.ScB);

    protected override (byte r, byte g, byte b) GetRgb()
        => (Color.R, Color.G, Color.B);
}