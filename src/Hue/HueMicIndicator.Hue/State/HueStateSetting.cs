using System.Collections.Generic;

namespace HueMicIndicator.Hue.State;

public record HueStateSetting(IReadOnlyDictionary<string, HueLightSetting> Lights)
{
    public static readonly HueStateSetting Empty = new(new Dictionary<string, HueLightSetting>());
}