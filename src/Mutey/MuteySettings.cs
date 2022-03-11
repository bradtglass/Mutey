namespace Mutey
{
    using Mutey.Core.Settings;

    public record MuteySettings( string? LastDeviceId ) : SettingsBase
    {
        public override string Filename => "mutey";

        public MuteySettings() : this( (string?) null ) { }
    }
}
