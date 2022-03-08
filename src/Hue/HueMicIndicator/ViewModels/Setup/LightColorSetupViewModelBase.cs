using HueMicIndicator.Hue.State.Color;

namespace HueMicIndicator.ViewModels.Setup;

public abstract class LightColorSetupViewModelBase : LightFieldSetupViewModel, IAffectsColor
{
    protected LightColorSetupViewModelBase(LightField field) : base(field) { }

    public override bool ConflictsWith(LightFieldSetupViewModel other)
        => other is LightColorSetupViewModelBase;

    public abstract HueColor GetHueColor();

    protected abstract (byte r, byte g, byte b) GetRgb();

    public (byte? a, (byte r, byte g, byte b)?) GetColorComponents()
        => (null, GetRgb());
}