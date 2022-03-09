namespace Mutey.Hue.ViewModels.Setup
{
    using Mutey.Hue.Client.State.Color;

    public abstract class LightColorSetupViewModelBase : LightFieldSetupViewModel, IAffectsColor
    {
        protected LightColorSetupViewModelBase( LightField field ) : base( field ) { }

        public (byte? a, (byte r, byte g, byte b)?) GetColorComponents()
        {
            return ( null, GetRgb() );
        }

        public override bool ConflictsWith( LightFieldSetupViewModel other )
        {
            return other is LightColorSetupViewModelBase;
        }

        public abstract HueColor GetHueColor();

        protected abstract (byte r, byte g, byte b) GetRgb();
    }
}
