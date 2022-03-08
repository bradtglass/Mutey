using System.Collections.Immutable;
using Mutey.Hue.Client.State;

namespace Mutey.Hue.Client;

using Mutey.Hue.Client.State;

public record HueSettings
{
    internal const string Sub = "hue";

    public string? AppKey { get; init; }

    public ImmutableDictionary<string, HueStateSetting> States { get; set; } =
        ImmutableDictionary<string, HueStateSetting>.Empty;
}