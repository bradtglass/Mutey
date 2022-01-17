using System.Collections.Generic;

namespace HueMicIndicator.Hue.State;

public record HueStateSetting(IReadOnlyDictionary<string,HueLightSetting> Lights);