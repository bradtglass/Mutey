namespace Mutey.Hue.Client
{
    using Q42.HueApi;

    public record LightInfo( string Id, string Name, LightCapabilities Capabilities );
}
