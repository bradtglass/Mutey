namespace Mutey.Hue.ViewModels.Setup
{
    using Mutey.Hue.Client.State.Color;

    public class LightColorTempSetupViewModel : LightColorSetupViewModelBase
    {
        private double temperature = 2500;

        public double Temperature
        {
            get => temperature;
            set => SetProperty( ref temperature, value );
        }

        public LightColorTempSetupViewModel() : base( LightField.ColorTemperature ) { }

        public override HueColor GetHueColor()
        {
            return new TemperatureHueColor( Temperature );
        }

        protected override (byte r, byte g, byte b) GetRgb()
        {
            var color = ColorUtility.TempToColor( Temperature );

            return ( color.R, color.G, color.B );
        }
    }
}
