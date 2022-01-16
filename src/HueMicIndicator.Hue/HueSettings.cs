using System.Collections.Immutable;

namespace HueMicIndicator.Hue;

public record HueSettings
{
    internal const string Sub = "hue";
    
    public string? AppKey { get; init; }
    
    public ImmutableDictionary<string, HueStateSetting> States { get; set; } = ImmutableDictionary<string, HueStateSetting>.Empty;
}