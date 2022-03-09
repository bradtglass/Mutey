namespace Mutey.Hue.Client.State.Color
{
    using System.Text.Json.Serialization;
    using Q42.HueApi;
    using Q42.HueApi.ColorConverters;
    using Q42.HueApi.ColorConverters.HSB;

    [ JsonConverter( typeof( HueColorConverter ) ) ]
    public sealed class RgbHueColor : HueColor
    {
        public RGBColor Color { get; }
        public RgbHueColor( double r, double g, double b ) : this( new RGBColor( r, g, b ) ) { }

        public RgbHueColor( RGBColor color )
        {
            Color = color;
        }

        public override void Apply( LightCommand command )
        {
            command.SetColor( Color );
        }

        private bool Equals( RgbHueColor other )
        {
            return Color.Equals( other.Color );
        }

        public override bool Equals( object? obj )
        {
            if ( ReferenceEquals( null, obj ) ) return false;
            if ( ReferenceEquals( this, obj ) ) return true;
            if ( obj.GetType() != GetType() ) return false;
            return Equals( (RgbHueColor) obj );
        }

        public override int GetHashCode()
        {
            return Color.GetHashCode();
        }
    }
}
