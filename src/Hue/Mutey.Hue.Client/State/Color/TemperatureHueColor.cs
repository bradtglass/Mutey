namespace Mutey.Hue.Client.State.Color
{
    using System.Text.Json.Serialization;
    using Q42.HueApi;

    [ JsonConverter( typeof( HueColorConverter ) ) ]
    public sealed class TemperatureHueColor : HueColor
    {
        /// <summary>
        ///     The temperature, in Kelvin.
        /// </summary>
        public double Temperature { get; }

        public TemperatureHueColor( double temperature )
        {
            Temperature = temperature;
        }

        private bool Equals( TemperatureHueColor other )
        {
            return Temperature.Equals( other.Temperature );
        }

        public override bool Equals( object? obj )
        {
            if ( ReferenceEquals( null, obj ) ) return false;
            if ( ReferenceEquals( this, obj ) ) return true;
            if ( obj.GetType() != GetType() ) return false;
            return Equals( (TemperatureHueColor) obj );
        }

        public override int GetHashCode()
        {
            return Temperature.GetHashCode();
        }

        public override void Apply( LightCommand command )
        {
            command.ColorTemperature = (int?) ( 1e6 / Temperature );
        }
    }
}
