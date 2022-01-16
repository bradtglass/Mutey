using System.Collections.Generic;
using System.Threading.Tasks;
using Q42.HueApi;
using Q42.HueApi.ColorConverters.Original;
using Q42.HueApi.Interfaces;

namespace HueMicIndicator.Hue;

public class HueLightState : IHueState
{
    private readonly IReadOnlyCollection<string> lights;
    private readonly HueLightSetting setting;

    public HueLightState(HueLightSetting setting, IReadOnlyCollection<string> lights)
    {
        this.setting = setting;
        this.lights = lights;
    }

    public async Task ApplyAsync(IHueClient client)
    {
        var command = new LightCommand();

        if (setting.Color.HasValue)
            command.SetColor(setting.Color.Value);

        command.On = setting.On;

        await client.SendCommandAsync(command, lights);
    }
}