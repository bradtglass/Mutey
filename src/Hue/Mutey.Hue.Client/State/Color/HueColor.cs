namespace Mutey.Hue.Client.State.Color
{
    using System.Text.Json.Serialization;
    using Q42.HueApi;

    [ JsonConverter( typeof( HueColorConverter ) ) ]
    public abstract class HueColor : IModifiesLightCommand
    {
        public abstract void Apply( LightCommand command );
    }
}
