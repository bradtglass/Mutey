namespace Mutey.Hue.Client.Test
{
    using System.Text.Json;
    using Mutey.Hue.Client.State.Color;
    using Q42.HueApi.ColorConverters;
    using Xunit;

    public class ColorSerializationTests
    {
        public static T RoundTrip<T>( T color )
            where T : HueColor
        {
            string json = JsonSerializer.Serialize( color );

            var color2 = JsonSerializer.Deserialize<HueColor>( json );

            Assert.NotNull( color2 );
            Assert.IsType<T>( color2 );

            return (T) color2!;
        }

        [ Fact ]
        public void CanRoundTripRgb()
        {
            RGBColor rgb = new(78, 56, 197);
            RgbHueColor color = new(rgb);

            var color2 = RoundTrip( color );

            Assert.Equal( rgb.R, color2.Color.R );
            Assert.Equal( rgb.G, color2.Color.G );
            Assert.Equal( rgb.B, color2.Color.B );
        }

        [ Fact ]
        public void CanRoundTripTemperature()
        {
            const double temp = 2345.32;
            TemperatureHueColor color = new(temp);

            var color2 = RoundTrip( color );

            Assert.Equal( temp, color2.Temperature, 1 );
        }

        [ Fact ]
        public void CanRoundTripXy()
        {
            const double x = 783.34312;
            const double y = 34.4565;
            XyHueColor color = new(x, y);

            var color2 = RoundTrip( color );

            Assert.Equal( x, color2.X );
            Assert.Equal( y, color2.Y );
        }
    }
}
