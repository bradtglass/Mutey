namespace Mutey.Hue.Client
{
    using System.Collections.Immutable;
    using Mutey.Core.Settings;
    using Mutey.Hue.Client.State;

    public record HueSettings : SettingsBase
    {
        public string? AppKey { get; init; }

        public ImmutableDictionary<string, HueStateSetting> States { get; set; } =
            ImmutableDictionary<string, HueStateSetting>.Empty;

        public override string Filename => "hue";
    }
}
