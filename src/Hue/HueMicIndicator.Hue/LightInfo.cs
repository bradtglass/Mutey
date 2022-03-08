using Q42.HueApi;

namespace HueMicIndicator.Hue;

public record LightInfo(string Id, string Name, LightCapabilities Capabilities);