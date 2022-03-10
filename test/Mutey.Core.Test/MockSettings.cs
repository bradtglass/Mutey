namespace Mutey.Core.Test
{
    using Mutey.Core.Settings;

    public record MockSettings( string Value ) : SettingsBase
    {
        public const string InitialValue = "Hello";

        public override string Filename => "mock";

        public MockSettings() : this( InitialValue ) { }
    }
}
