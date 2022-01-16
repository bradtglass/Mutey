using System.Collections.Generic;

namespace HueMicIndicator.Hue;

public record HueStateSetting(IReadOnlyDictionary<string,HueLightSetting> Lights);