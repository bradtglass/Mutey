namespace Mutey.Hue.Client.State.Color
{
    using System;
    using System.Text.Json.Serialization;
    using Q42.HueApi;

    [ JsonConverter( typeof( HueColorConverter ) ) ]
    public sealed class XyHueColor : HueColor
    {
        public double X { get; }

        public double Y { get; }

        public XyHueColor( double x, double y )
        {
            X = x;
            Y = y;
        }

        private bool Equals( XyHueColor other )
        {
            return X.Equals( other.X ) && Y.Equals( other.Y );
        }

        public override bool Equals( object? obj )
        {
            if ( ReferenceEquals( null, obj ) ) return false;
            if ( ReferenceEquals( this, obj ) ) return true;
            if ( obj.GetType() != GetType() ) return false;
            return Equals( (XyHueColor) obj );
        }

        public override int GetHashCode()
        {
            return HashCode.Combine( X, Y );
        }

        public override void Apply( LightCommand command )
        {
            command.SetColor( X, Y );
        }
    }
}
