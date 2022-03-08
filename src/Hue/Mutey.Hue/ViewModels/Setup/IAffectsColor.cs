namespace Mutey.Hue.ViewModels.Setup;

public interface IAffectsColor
{
    (byte? a, (byte r, byte g, byte b)?) GetColorComponents();
}