namespace Mutey.Hue.ViewModels.Setup
{
    public class LightBrightnessSetupViewModel : LightFieldSetupViewModel, IAffectsColor
    {
        private double brightness = 1;

        public double Brightness
        {
            get => brightness;
            set => SetProperty( ref brightness, value );
        }

        public LightBrightnessSetupViewModel() : base( LightField.Brightness ) { }

        public (byte? a, (byte r, byte g, byte b)?) GetColorComponents()
        {
            return ( GetBrightness(), null );
        }

        public void SetBrightness( byte value )
        {
            Brightness = (double) value / byte.MaxValue;
        }

        public byte GetBrightness()
        {
            return (byte) ( Brightness * byte.MaxValue );
        }
    }
}
