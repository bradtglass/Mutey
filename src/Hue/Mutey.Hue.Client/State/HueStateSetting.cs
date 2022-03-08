using System.Collections.Generic;

namespace Mutey.Hue.Client.State;

public record HueStateSetting(IReadOnlyDictionary<string, HueLightSetting> Lights)
{
    public static readonly HueStateSetting Empty = new(new Dictionary<string, HueLightSetting>());
}