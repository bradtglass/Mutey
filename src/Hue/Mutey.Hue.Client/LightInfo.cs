using Q42.HueApi;

namespace Mutey.Hue.Client;

public record LightInfo(string Id, string Name, LightCapabilities Capabilities);