namespace Mutey.Hue.ViewModels.Setup
{
    using System.Windows.Media;
    using Mutey.Hue.Client.State.Color;

    public class LightColorSetupViewModel : LightColorSetupViewModelBase
    {
        private Color color = Colors.MediumBlue;

        public Color Color
        {
            get => color;
            set => SetProperty( ref color, value );
        }

        public LightColorSetupViewModel() : base( LightField.Color ) { }

        public override HueColor GetHueColor()
        {
            return new RgbHueColor( Color.ScR, Color.ScG, Color.ScB );
        }

        protected override (byte r, byte g, byte b) GetRgb()
        {
            return ( Color.R, Color.G, Color.B );
        }
    }
}
