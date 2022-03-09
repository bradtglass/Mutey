namespace Mutey.Hue.Client.State.Color
{
    using Q42.HueApi;

    public interface IModifiesLightCommand
    {
        void Apply( LightCommand command );
    }
}
